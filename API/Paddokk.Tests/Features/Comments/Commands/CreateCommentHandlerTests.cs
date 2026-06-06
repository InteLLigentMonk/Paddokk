using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Commands.CreateComment;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Comments.Commands;

public class CreateCommentHandlerTests
{
    private const int PostId = 100;
    private const string PostOwnerId = "owner-1";
    private const string VisitorId = "visitor-1";

    private readonly ICommentRepository _comments = Substitute.For<ICommentRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly CreateCommentHandler _handler;

    public CreateCommentHandlerTests()
    {
        _comments.JourneyPostExists(PostId, Arg.Any<CancellationToken>()).Returns(true);
        _comments.UserExist(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        _comments.When(r => r.CreateCommentAsync(Arg.Any<PostComment>(), Arg.Any<CancellationToken>()))
            .Do(ci => ci.ArgAt<PostComment>(0).Id = 42);

        _comments.GetCommentByIdAsync(42, Arg.Any<CancellationToken>())
            .Returns(ci => CommentTestHelpers.BuildComment(42, PostId, _actor.UserId, PostOwnerId));

        _handler = new CreateCommentHandler(_comments, _actor, _publisher, NullLogger<CreateCommentHandler>.Instance);
    }

    [Fact]
    public async Task Handle_PostNotFound_ReturnsNotFound()
    {
        _comments.JourneyPostExists(PostId, Arg.Any<CancellationToken>()).Returns(false);
        _actor.UserId.Returns(VisitorId);

        var result = await _handler.Handle(new CreateCommentCommand(PostId, "hello"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns(VisitorId);
        _comments.UserExist(VisitorId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new CreateCommentCommand(PostId, "hello"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_TopLevelComment_PersistsAndReturnsDto()
    {
        _actor.UserId.Returns(VisitorId);

        PostComment? captured = null;
        _comments.When(r => r.CreateCommentAsync(Arg.Any<PostComment>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var c = ci.ArgAt<PostComment>(0);
                c.Id = 42;
                captured = c;
            });

        var result = await _handler.Handle(new CreateCommentCommand(PostId, "  hello  "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.AuthorId.Should().Be(VisitorId);
        captured.JourneyPostId.Should().Be(PostId);
        captured.Content.Should().Be("hello");
        captured.ParentCommentId.Should().BeNull();
        await _publisher.Received(1).Publish(
            Arg.Is<CommentedOnPost>(e => e.ActorId == VisitorId && e.PostId == PostId && e.PostAuthorId == PostOwnerId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReplyToMissingParent_ReturnsNotFound()
    {
        _actor.UserId.Returns(PostOwnerId);
        _comments.GetCommentByIdAsync(99, Arg.Any<CancellationToken>()).Returns((PostComment?)null);

        var result = await _handler.Handle(
            new CreateCommentCommand(PostId, "hi", ParentCommentId: 99),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ReplyToParentOnDifferentPost_ReturnsNotFound()
    {
        _actor.UserId.Returns(PostOwnerId);
        var parent = CommentTestHelpers.BuildComment(id: 7, postId: 999, authorId: VisitorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(7, Arg.Any<CancellationToken>()).Returns(parent);

        var result = await _handler.Handle(
            new CreateCommentCommand(PostId, "hi", ParentCommentId: 7),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ReplyToAReply_ReturnsValidation_PerAdr0002()
    {
        _actor.UserId.Returns(PostOwnerId);
        var parentReply = CommentTestHelpers.BuildComment(
            id: 7, postId: PostId, authorId: PostOwnerId, postAuthorId: PostOwnerId, parentCommentId: 1);
        _comments.GetCommentByIdAsync(7, Arg.Any<CancellationToken>()).Returns(parentReply);

        var result = await _handler.Handle(
            new CreateCommentCommand(PostId, "hi", ParentCommentId: 7),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Message.Should().Contain("reply to a reply");
    }

    [Fact]
    public async Task Handle_NonOwnerTriesToReply_ReturnsUnauthorized_PerAdr0002()
    {
        _actor.UserId.Returns(VisitorId);
        var parent = CommentTestHelpers.BuildComment(id: 7, postId: PostId, authorId: VisitorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(7, Arg.Any<CancellationToken>()).Returns(parent);

        var result = await _handler.Handle(
            new CreateCommentCommand(PostId, "hi", ParentCommentId: 7),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Message.Should().Contain("post owner");
    }

    [Fact]
    public async Task Handle_PostOwnerReplies_PersistsWithParentCommentId()
    {
        _actor.UserId.Returns(PostOwnerId);
        var parent = CommentTestHelpers.BuildComment(id: 7, postId: PostId, authorId: VisitorId, postAuthorId: PostOwnerId);
        _comments.GetCommentByIdAsync(7, Arg.Any<CancellationToken>()).Returns(parent);

        PostComment? captured = null;
        _comments.When(r => r.CreateCommentAsync(Arg.Any<PostComment>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var c = ci.ArgAt<PostComment>(0);
                c.Id = 42;
                captured = c;
            });
        _comments.GetCommentByIdAsync(42, Arg.Any<CancellationToken>())
            .Returns(CommentTestHelpers.BuildComment(42, PostId, PostOwnerId, PostOwnerId, parentCommentId: 7));

        var result = await _handler.Handle(
            new CreateCommentCommand(PostId, "thanks!", ParentCommentId: 7),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.ParentCommentId.Should().Be(7);
        captured.AuthorId.Should().Be(PostOwnerId);
        // Replies route to the separate ReplyToYourComment flow, never CommentOnYourPost (ADR-0002).
        await _publisher.DidNotReceive().Publish(Arg.Any<CommentedOnPost>(), Arg.Any<CancellationToken>());
    }
}
