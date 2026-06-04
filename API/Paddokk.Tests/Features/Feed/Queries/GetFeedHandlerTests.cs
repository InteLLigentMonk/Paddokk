using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Feed.Queries.GetFeed;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Tests.Features.Feed.Queries;

// Per ADR-0001 these tests mock the IFeedRepository seam. The handler owns only actor
// resolution, the empty-graph contract, and pagination wiring — the SQL UNION semantics
// (dedup, ordering, source paths, visibility) live behind the seam and are deliberately
// NOT exercised here (see #185: semantics verification deferred, no integration suite).
public class GetFeedHandlerTests
{
    private readonly IFeedRepository _repo = Substitute.For<IFeedRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetFeedHandler _handler;

    public GetFeedHandlerTests()
    {
        _handler = new GetFeedHandler(_repo, _actor);
    }

    private static FeedItemDto BuildJourneyPostItem(int postId, DateTime createdAt) => new()
    {
        Type = FeedItemType.JourneyPost,
        CreatedAt = createdAt,
        ActorUsername = "test.user",
        ActorDisplayName = "Test User",
        JourneyId = 1,
        JourneyTitle = "Engine swap",
        JourneySlug = "engine-swap",
        UserCarId = 1,
        UserCarSlug = "73-capri",
        UserCarLabel = "'73 Capri",
        JourneyPostId = postId,
        TextContent = "post body",
        ImageUrls = [],
        CommentCount = 0
    };

    private void AuthenticatedAs(string userId)
    {
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns(userId);
    }

    [Fact]
    public async Task Handle_AuthenticatedActor_QueriesRepositoryWithActorId()
    {
        AuthenticatedAs("actor-1");
        _repo.GetFeedAsync("actor-1", 1, 20, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<FeedItemDto>)[BuildJourneyPostItem(10, DateTime.UtcNow)], 1));

        await _handler.Handle(new GetFeedQuery(), CancellationToken.None);

        await _repo.Received(1).GetFeedAsync("actor-1", 1, 20, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AuthenticatedActor_ReturnsPagedFeedItems()
    {
        AuthenticatedAs("actor-1");
        var item = BuildJourneyPostItem(10, DateTime.UtcNow);
        _repo.GetFeedAsync("actor-1", 1, 20, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<FeedItemDto>)[item], 1));

        var result = await _handler.Handle(new GetFeedQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().ContainSingle().Which.Should().Be(item);
        result.Value.TotalCount.Should().Be(1);
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_EmptyGraph_ReturnsEmptyPage_WithNoFallbackContent()
    {
        AuthenticatedAs("lonely-actor");
        _repo.GetFeedAsync("lonely-actor", 1, 20, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<FeedItemDto>)[], 0));

        var result = await _handler.Handle(new GetFeedQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_MorePagesRemain_ReportsHasNextPage()
    {
        AuthenticatedAs("actor-1");
        var page = Enumerable.Range(1, 20)
            .Select(i => BuildJourneyPostItem(i, DateTime.UtcNow.AddMinutes(-i)))
            .ToList();
        _repo.GetFeedAsync("actor-1", 1, 20, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<FeedItemDto>)page, 25));

        var result = await _handler.Handle(new GetFeedQuery(Page: 1, PageSize: 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeTrue();
        result.Value.TotalCount.Should().Be(25);
    }

    [Fact]
    public async Task Handle_SecondPage_PassesPagingThroughToRepository()
    {
        AuthenticatedAs("actor-1");
        _repo.GetFeedAsync("actor-1", 2, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<FeedItemDto>)[], 10));

        var result = await _handler.Handle(new GetFeedQuery(Page: 2, PageSize: 10), CancellationToken.None);

        await _repo.Received(1).GetFeedAsync("actor-1", 2, 10, Arg.Any<CancellationToken>());
        result.Value!.Page.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_UnauthenticatedActor_ReturnsUnauthorized_AndNeverQueriesRepository()
    {
        _actor.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetFeedQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        await _repo.DidNotReceiveWithAnyArgs().GetFeedAsync(default!, default, default, default);
    }
}
