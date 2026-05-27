using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Queries.GetPostComments;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Comments.Queries;

public class GetPostCommentsHandlerTests
{
    private readonly ICommentRepository _comments = Substitute.For<ICommentRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetPostCommentsHandler _handler;

    public GetPostCommentsHandlerTests()
    {
        _handler = new GetPostCommentsHandler(_comments, _actor);
    }

    private static PostComment BuildComment(int id, int postId, string authorId, string postAuthorId = "owner-1") => new()
    {
        Id = id,
        JourneyPostId = postId,
        AuthorId = authorId,
        Content = "hi",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Author = JourneyTestHelpers.BuildUser(authorId),
        JourneyPost = new JourneyPost
        {
            Id = postId,
            JourneyId = 1,
            AuthorId = postAuthorId,
            Author = JourneyTestHelpers.BuildUser(postAuthorId)
        },
        Replies = []
    };

    private void SeedComments(int postId, params PostComment[] postComments)
    {
        _comments.GetPostCommentsAsync(postId, Arg.Any<CancellationToken>(), 1, 20)
            .Returns((postComments.ToList(), postComments.Length));
    }

    [Fact]
    public async Task Handle_PrivateJourney_AnonymousActor_ReturnsEmpty()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _comments.GetParentJourneyAsync(100, Arg.Any<CancellationToken>()).Returns(journey);
        SeedComments(100, BuildComment(1, 100, "owner-1"));
        _actor.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetPostCommentsQuery(100), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PrivateJourney_NonOwnerActor_ReturnsEmpty()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _comments.GetParentJourneyAsync(100, Arg.Any<CancellationToken>()).Returns(journey);
        SeedComments(100, BuildComment(1, 100, "owner-1"));
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("visitor-1");

        var result = await _handler.Handle(new GetPostCommentsQuery(100), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PrivateJourney_OwnerActor_ReturnsFullList()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _comments.GetParentJourneyAsync(100, Arg.Any<CancellationToken>()).Returns(journey);
        SeedComments(100, BuildComment(1, 100, "owner-1"), BuildComment(2, 100, "owner-1"));
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("owner-1");

        var result = await _handler.Handle(new GetPostCommentsQuery(100), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_PublicJourney_AnonymousActor_ReturnsFullList()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = true;
        _comments.GetParentJourneyAsync(100, Arg.Any<CancellationToken>()).Returns(journey);
        SeedComments(100, BuildComment(1, 100, "owner-1"));
        _actor.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetPostCommentsQuery(100), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_PostNotFound_ReturnsNotFound()
    {
        _comments.GetParentJourneyAsync(999, Arg.Any<CancellationToken>()).Returns((Journey?)null);

        var result = await _handler.Handle(new GetPostCommentsQuery(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(Paddokk.Core.Models.ErrorType.NotFound);
    }
}