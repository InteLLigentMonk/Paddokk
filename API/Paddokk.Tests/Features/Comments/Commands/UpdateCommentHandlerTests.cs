using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Commands.UpdateComment;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Comments.Commands;

public class UpdateCommentHandlerTests
{
    private const int CommentId = 7;
    private const int PostId = 100;
    private const string AuthorId = "author-1";
    private const string OtherId = "other-1";

    private readonly ICommentRepository _comments = Substitute.For<ICommentRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UpdateCommentHandler _handler;

    public UpdateCommentHandlerTests()
    {
        _handler = new UpdateCommentHandler(_comments, _actor, NullLogger<UpdateCommentHandler>.Instance);
    }

    [Fact]
    public async Task Handle_CommentNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns(AuthorId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns((PostComment?)null);

        var result = await _handler.Handle(new UpdateCommentCommand(CommentId, "updated"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_NonAuthor_ReturnsUnauthorized()
    {
        _actor.UserId.Returns(OtherId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new UpdateCommentCommand(CommentId, "updated"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        await _comments.DidNotReceive().UpdateCommentAsync(
            Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AuthorUpdates_PersistsAndReturnsDto()
    {
        _actor.UserId.Returns(AuthorId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);
        _comments.UpdateCommentAsync(AuthorId, CommentId, "updated", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new UpdateCommentCommand(CommentId, "updated"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(CommentId);
        await _comments.Received(1).UpdateCommentAsync(AuthorId, CommentId, "updated", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UpdateRepoReturnsFalse_ReturnsNotFound()
    {
        _actor.UserId.Returns(AuthorId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);
        _comments.UpdateCommentAsync(AuthorId, CommentId, "updated", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new UpdateCommentCommand(CommentId, "updated"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
