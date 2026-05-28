using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Commands.DeleteComment;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Comments.Commands;

public class DeleteCommentHandlerTests
{
    private const int CommentId = 7;
    private const int PostId = 100;
    private const string AuthorId = "author-1";
    private const string PostOwnerId = "owner-1";
    private const string OtherId = "other-1";

    private readonly ICommentRepository _comments = Substitute.For<ICommentRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly DeleteCommentHandler _handler;

    public DeleteCommentHandlerTests()
    {
        _handler = new DeleteCommentHandler(_comments, _actor, NullLogger<DeleteCommentHandler>.Instance);
    }

    [Fact]
    public async Task Handle_CommentNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns(AuthorId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns((PostComment?)null);

        var result = await _handler.Handle(new DeleteCommentCommand(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_AuthorDeletes_Succeeds()
    {
        _actor.UserId.Returns(AuthorId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new DeleteCommentCommand(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _comments.Received(1).DeleteCommentAsync(comment, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PostOwnerDeletesAnotherUsersComment_Succeeds()
    {
        _actor.UserId.Returns(PostOwnerId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: OtherId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new DeleteCommentCommand(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _comments.Received(1).DeleteCommentAsync(comment, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThirdPartyTriesToDelete_ReturnsUnauthorized()
    {
        _actor.UserId.Returns(OtherId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new DeleteCommentCommand(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        await _comments.DidNotReceive().DeleteCommentAsync(Arg.Any<PostComment>(), Arg.Any<CancellationToken>());
    }
}
