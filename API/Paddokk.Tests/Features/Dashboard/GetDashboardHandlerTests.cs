using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Dashboard;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Dashboard;

public class GetDashboardHandlerTests
{
    private const string UserId = "user-1";

    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IJourneyRepository _journeyRepo = Substitute.For<IJourneyRepository>();
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetDashboardHandler _handler;

    public GetDashboardHandlerTests()
    {
        _actor.UserId.Returns(UserId);

        _userRepo.GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildUser(UserId));
        _journeyRepo.GetUserAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildUser(UserId));
        _journeyRepo.GetUserJourneysWithStatsAsync(UserId, Arg.Any<CancellationToken>())
            .Returns([]);
        _journeyRepo.GetUserJourneysAsync(UserId, Arg.Any<CancellationToken>())
            .Returns([]);
        _carRepo.GetUserCarsAsync(UserId, Arg.Any<CancellationToken>())
            .Returns([]);
        _carRepo.GetUserCarCountAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(0);

        _handler = new GetDashboardHandler(_userRepo, _journeyRepo, _carRepo, _actor);
    }

    private static Journey BuildJourneyWithStats(
        int id,
        JourneyStatus status,
        int posts = 0,
        int activeSubs = 0,
        int inactiveSubs = 0,
        int likes = 0,
        DateTime? updatedAt = null)
    {
        var j = JourneyTestHelpers.BuildJourney(id: id, userId: UserId);
        j.Status = status;
        j.UpdatedAt = updatedAt ?? DateTime.UtcNow;
        j.Posts = Enumerable.Range(0, posts).Select(_ => new JourneyPost
        {
            JourneyId = id,
            AuthorId = UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();
        j.Subscriptions =
        [
            .. Enumerable.Range(0, activeSubs).Select(i => new JourneySubscription
            {
                JourneyId = id,
                UserId = $"sub-active-{id}-{i}",
                IsActive = true
            }),
            .. Enumerable.Range(0, inactiveSubs).Select(i => new JourneySubscription
            {
                JourneyId = id,
                UserId = $"sub-inactive-{id}-{i}",
                IsActive = false
            })
        ];
        j.Likes = [.. Enumerable.Range(0, likes).Select(i => new JourneyLike
        {
            JourneyId = id,
            UserId = $"liker-{id}-{i}"
        })];
        return j;
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        _userRepo.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_AggregatesStatsByStatusAndSumsChildren()
    {
        _journeyRepo.GetUserJourneysWithStatsAsync(UserId, Arg.Any<CancellationToken>()).Returns(
        [
            BuildJourneyWithStats(1, JourneyStatus.Active, posts: 3, activeSubs: 2, inactiveSubs: 1, likes: 5),
            BuildJourneyWithStats(2, JourneyStatus.Active, posts: 1, activeSubs: 0, inactiveSubs: 0, likes: 0),
            BuildJourneyWithStats(3, JourneyStatus.Completed, posts: 4, activeSubs: 1, inactiveSubs: 0, likes: 2)
        ]);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var stats = result.Value!.Stats;
        stats.TotalJourneys.Should().Be(3);
        stats.ActiveJourneys.Should().Be(2);
        stats.CompletedJourneys.Should().Be(1);
        stats.TotalPosts.Should().Be(8);
        stats.TotalSubscribers.Should().Be(3);
        stats.TotalLikes.Should().Be(7);
    }

    [Fact]
    public async Task Handle_NoJourneys_StatsAreZero()
    {
        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var stats = result.Value!.Stats;
        stats.TotalJourneys.Should().Be(0);
        stats.ActiveJourneys.Should().Be(0);
        stats.CompletedJourneys.Should().Be(0);
        stats.TotalPosts.Should().Be(0);
        stats.TotalSubscribers.Should().Be(0);
        stats.TotalLikes.Should().Be(0);
    }

    [Fact]
    public async Task Handle_RecentJourneys_OnlyActive_SortedDesc_MaxFive()
    {
        var now = DateTime.UtcNow;
        var journeys = new List<Journey>
        {
            BuildJourneyWithStats(1, JourneyStatus.Active, updatedAt: now.AddDays(-1)),
            BuildJourneyWithStats(2, JourneyStatus.Active, updatedAt: now.AddDays(-2)),
            BuildJourneyWithStats(3, JourneyStatus.Active, updatedAt: now.AddDays(-3)),
            BuildJourneyWithStats(4, JourneyStatus.Active, updatedAt: now.AddDays(-4)),
            BuildJourneyWithStats(5, JourneyStatus.Active, updatedAt: now.AddDays(-5)),
            BuildJourneyWithStats(6, JourneyStatus.Active, updatedAt: now.AddDays(-6)),
            BuildJourneyWithStats(7, JourneyStatus.Completed, updatedAt: now)
        };
        _journeyRepo.GetUserJourneysAsync(UserId, Arg.Any<CancellationToken>()).Returns(journeys);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var recent = result.Value!.RecentJourneys.ToList();
        recent.Should().HaveCount(5);
        recent.Select(r => r.Id).Should().Equal(1, 2, 3, 4, 5);
        recent.Should().OnlyContain(r => r.Status == JourneyStatus.Active);
    }

    [Fact]
    public async Task Handle_DefaultActiveJourney_NullWhenUserHasNone()
    {
        var user = JourneyTestHelpers.BuildUser(UserId);
        user.DefaultActiveJourneyId = null;
        _journeyRepo.GetUserAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DefaultActiveJourney.Should().BeNull();
        result.Value.QuickActions.HasDefaultJourney.Should().BeFalse();
        result.Value.QuickActions.CanCreatePost.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DefaultActiveJourney_PopulatedWhenSet()
    {
        var user = JourneyTestHelpers.BuildUser(UserId);
        user.DefaultActiveJourneyId = 42;
        _journeyRepo.GetUserAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);
        _journeyRepo.GetJourneyByIdAsync(42, Arg.Any<CancellationToken>())
            .Returns(BuildJourneyWithStats(42, JourneyStatus.Active));

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DefaultActiveJourney.Should().NotBeNull();
        result.Value.DefaultActiveJourney!.Id.Should().Be(42);
        result.Value.QuickActions.HasDefaultJourney.Should().BeTrue();
        result.Value.QuickActions.CanCreatePost.Should().BeTrue();
    }

    [Theory]
    [InlineData(SubscriptionTier.Free, "1", "1")]
    [InlineData(SubscriptionTier.Silver, "3", "3")]
    [InlineData(SubscriptionTier.Gold, "10", "10")]
    [InlineData(SubscriptionTier.Platinum, "20", "20")]
    [InlineData(SubscriptionTier.Diamond, "Unlimited", "Unlimited")]
    public async Task Handle_LimitsReflectSubscriptionTier(SubscriptionTier tier, string expectedMaxCars, string expectedMaxJourneys)
    {
        var user = JourneyTestHelpers.BuildUser(UserId);
        user.SubscriptionTier = tier;
        _userRepo.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Limits.Cars.Max.Should().Be(expectedMaxCars);
        result.Value.Limits.Journeys.Max.Should().Be(expectedMaxJourneys);
    }

    [Fact]
    public async Task Handle_CarCountAtLimit_CanAddCarFalse()
    {
        var user = JourneyTestHelpers.BuildUser(UserId);
        user.SubscriptionTier = SubscriptionTier.Free;
        _userRepo.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);
        _carRepo.GetUserCarCountAsync(UserId, Arg.Any<CancellationToken>()).Returns(1);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Limits.Cars.CanAdd.Should().BeFalse();
        result.Value.QuickActions.CanAddCar.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NoCars_NeedsCarRegistrationIsTrue()
    {
        _carRepo.GetUserCarsAsync(UserId, Arg.Any<CancellationToken>()).Returns([]);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.QuickActions.NeedsCarRegistration.Should().BeTrue();
        result.Value.Limits.Cars.Current.Should().Be(0);
    }

    [Fact]
    public async Task Handle_HasCars_NeedsCarRegistrationIsFalse()
    {
        var cars = new List<UserCar>
        {
            new() { Id = 1, PrincipalId = UserId, Slug = "c-1" },
            new() { Id = 2, PrincipalId = UserId, Slug = "c-2" }
        };
        _carRepo.GetUserCarsAsync(UserId, Arg.Any<CancellationToken>()).Returns(cars);

        var result = await _handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.QuickActions.NeedsCarRegistration.Should().BeFalse();
        result.Value.Limits.Cars.Current.Should().Be(2);
    }
}
