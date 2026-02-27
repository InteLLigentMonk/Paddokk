using MediatR;
using Paddokk.Core.Features.Journeys;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Dashboard;

public sealed class GetDashboardHandler(
    IUserRepository userRepository,
    IJourneyRepository journeyRepository,
    ICarRepository carRepository,
    IActorResolver actor)
    : IRequestHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    public async Task<Result<DashboardResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken);
        if (user is null)
            return Result<DashboardResponse>.Failure(Error.NotFound("User not found"));

        var journeyStatsTask = journeyRepository.GetUserJourneysWithStatsAsync(actor.UserId, cancellationToken);
        var carsTask = carRepository.GetUserCarsAsync(actor.UserId, cancellationToken);
        var userJourneysTask = journeyRepository.GetUserJourneysAsync(actor.UserId, cancellationToken);
        var defaultJourneyUserTask = journeyRepository.GetUserAsync(actor.UserId, cancellationToken);
        var carCountTask = carRepository.GetUserCarCountAsync(actor.UserId, cancellationToken);

        await Task.WhenAll(journeyStatsTask, carsTask, userJourneysTask, defaultJourneyUserTask, carCountTask);

        var journeyStatsData = await journeyStatsTask;
        var carCount = (await carsTask).Count;
        var allJourneys = await userJourneysTask;
        var userWithDefault = await defaultJourneyUserTask;
        var currentCarCount = await carCountTask;

        var journeyStats = new JourneyStatsDto
        {
            TotalJourneys = journeyStatsData.Count,
            ActiveJourneys = journeyStatsData.Count(j => j.Status == JourneyStatus.Active),
            CompletedJourneys = journeyStatsData.Count(j => j.Status == JourneyStatus.Completed),
            TotalPosts = journeyStatsData.Sum(j => j.Posts.Count),
            TotalSubscribers = journeyStatsData.Sum(j => j.Subscriptions.Count(s => s.IsActive)),
            TotalLikes = journeyStatsData.Sum(j => j.Likes.Count)
        };

        var recentJourneys = allJourneys
            .Where(j => j.Status == JourneyStatus.Active)
            .OrderByDescending(j => j.UpdatedAt)
            .Take(5)
            .Select(j => JourneyMapping.ToJourneyDto(j, actor.UserId));

        JourneyDto? defaultActiveJourney = null;
        if (userWithDefault?.DefaultActiveJourneyId is not null)
        {
            var defaultJourney = await journeyRepository.GetJourneyByIdAsync(
                userWithDefault.DefaultActiveJourneyId.Value, cancellationToken);
            if (defaultJourney is not null)
                defaultActiveJourney = JourneyMapping.ToJourneyDto(defaultJourney, actor.UserId);
        }

        var maxCars = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };

        var maxJourneys = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        var canAddCar = currentCarCount < maxCars;
        var canCreateJourney = journeyStats.TotalJourneys < maxJourneys;

        return Result<DashboardResponse>.Success(new DashboardResponse
        {
            User = new DashboardUser
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                SubscriptionTier = user.SubscriptionTier,
                EmailConfirmed = false
            },
            Stats = journeyStats,
            Limits = new DashboardLimits
            {
                Cars = new DashboardResourceLimit
                {
                    Current = carCount,
                    Max = maxCars == int.MaxValue ? "Unlimited" : maxCars.ToString(),
                    CanAdd = canAddCar
                },
                Journeys = new DashboardResourceLimit
                {
                    Current = journeyStats.TotalJourneys,
                    Max = maxJourneys == int.MaxValue ? "Unlimited" : maxJourneys.ToString(),
                    CanAdd = canCreateJourney
                }
            },
            RecentJourneys = recentJourneys,
            DefaultActiveJourney = defaultActiveJourney,
            QuickActions = new DashboardQuickActions
            {
                HasDefaultJourney = defaultActiveJourney is not null,
                CanCreatePost = defaultActiveJourney is not null,
                CanCreateJourney = canCreateJourney,
                CanAddCar = canAddCar,
                NeedsCarRegistration = carCount == 0
            }
        });
    }
}
