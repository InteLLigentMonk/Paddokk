using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Follows.Queries.GetFollowing;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Follows.Queries;

public class GetFollowingHandlerTests
{
    private readonly IUserFollowRepository _followRepo = Substitute.For<IUserFollowRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetFollowingHandler _handler;

    public GetFollowingHandlerTests()
    {
        _handler = new GetFollowingHandler(_followRepo, _actor);
    }

    private static UserProfileProjection Projection(string id, bool isFollowedByMe = false) =>
        new(
            new ApplicationUser { Id = id, Username = id, DisplayName = id, FirstName = "F", Email = $"{id}@x.com" },
            CarCount: 0,
            JourneyCount: 0,
            FollowerCount: 0,
            FollowingCount: 0,
            IsFollowedByMe: isFollowedByMe);

    private void SeedFollowing(int total, params UserProfileProjection[] items) =>
        _followRepo
            .GetFollowingAsync("u1", Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<UserProfileProjection>)items.ToList(), total));

    [Fact]
    public async Task Handle_MapsRowsToUserDtos_CarryingIsFollowedByMe()
    {
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("me");
        SeedFollowing(2, Projection("a", isFollowedByMe: true), Projection("b", isFollowedByMe: false));

        var result = await _handler.Handle(new GetFollowingQuery("u1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items[0].Id.Should().Be("a");
        result.Value.Items[0].IsFollowedByMe.Should().BeTrue();
        result.Value.Items[1].IsFollowedByMe.Should().BeFalse();
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_AuthenticatedActor_PassesActorUserIdToRepository()
    {
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("me");
        SeedFollowing(0);

        await _handler.Handle(new GetFollowingQuery("u1"), CancellationToken.None);

        await _followRepo.Received(1).GetFollowingAsync(
            "u1", "me", Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AnonymousActor_PassesNullActorUserIdToRepository()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(0);

        await _handler.Handle(new GetFollowingQuery("u1"), CancellationToken.None);

        await _followRepo.Received(1).GetFollowingAsync(
            "u1", null, Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PageBelowMinimum_NormalizesToFirstPage()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(0);

        var result = await _handler.Handle(new GetFollowingQuery("u1", Page: 0), CancellationToken.None);

        result.Value!.Page.Should().Be(PaginationDefaults.MinPage);
    }

    [Fact]
    public async Task Handle_PageSizeAboveMax_NormalizesToMax()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(0);

        var result = await _handler.Handle(
            new GetFollowingQuery("u1", PageSize: PaginationDefaults.MaxPageSize + 50), CancellationToken.None);

        result.Value!.PageSize.Should().Be(PaginationDefaults.MaxPageSize);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(0);

        var result = await _handler.Handle(new GetFollowingQuery("u1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ExactPageMultiple_LastPageHasNoNextPage()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(40, Projection("a"));

        var result = await _handler.Handle(
            new GetFollowingQuery("u1", Page: 2, PageSize: 20), CancellationToken.None);

        result.Value!.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_MorePagesRemaining_ReportsNextPage()
    {
        _actor.IsAuthenticated.Returns(false);
        SeedFollowing(40, Projection("a"));

        var result = await _handler.Handle(
            new GetFollowingQuery("u1", Page: 1, PageSize: 20), CancellationToken.None);

        result.Value!.HasNextPage.Should().BeTrue();
    }
}
