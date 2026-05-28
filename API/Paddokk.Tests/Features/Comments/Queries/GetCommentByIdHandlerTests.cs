using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Queries.GetCommentById;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Comments.Queries;

public class GetCommentByIdHandlerTests
{
    private const int CommentId = 7;
    private const int PostId = 100;
    private const string AuthorId = "author-1";
    private const string PostOwnerId = "owner-1";

    private readonly ICommentRepository _comments = Substitute.For<ICommentRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetCommentByIdHandler _handler;

    public GetCommentByIdHandlerTests()
    {
        _handler = new GetCommentByIdHandler(_comments, _actor);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNotFound()
    {
        _actor.IsAuthenticated.Returns(false);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns((PostComment?)null);

        var result = await _handler.Handle(new GetCommentByIdQuery(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_AnonymousActor_ReturnsDtoWithIsOwnerFalse()
    {
        _actor.IsAuthenticated.Returns(false);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new GetCommentByIdQuery(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(CommentId);
        result.Value.AuthorId.Should().Be(AuthorId);
        result.Value.IsOwner.Should().BeFalse();
        result.Value.IsViewerPostOwner.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_AuthorActor_ReturnsDtoWithIsOwnerTrue()
    {
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns(AuthorId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new GetCommentByIdQuery(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsOwner.Should().BeTrue();
        result.Value.IsViewerPostOwner.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_PostOwnerActor_ReturnsDtoWithIsViewerPostOwnerTrue()
    {
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns(PostOwnerId);
        var comment = CommentTestHelpers.BuildComment(CommentId, PostId, authorId: AuthorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(CommentId, Arg.Any<CancellationToken>()).Returns(comment);

        var result = await _handler.Handle(new GetCommentByIdQuery(CommentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsViewerPostOwner.Should().BeTrue();
        result.Value.IsOwner.Should().BeFalse();
    }
}
