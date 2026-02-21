using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IJourneyService _journeyService;
    private readonly ICarService _carService;
    private readonly IUserService _userService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IJourneyService journeyService,
        ICarService carService,
        IUserService userService,
        ILogger<DashboardController> logger)
    {
        _journeyService = journeyService;
        _carService = carService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard data for authenticated user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetDashboard(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var subTier = User.GetSubscriptionTier();

        // Get user info
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        // Get user's journey stats
        var journeyStats = await _journeyService.GetUserJourneyStatsAsync(userId);

        // Get user's cars
        var cars = await _carService.GetUserCarsAsync(userId, cancellationToken);
        var carCount = cars.Count();

        // Get user's active journeys (recent activity)
        var activeJourneys = await _journeyService.GetUserJourneysAsync(userId, userId);
        var recentJourneys = activeJourneys
            .Where(j => j.Status == JourneyStatus.Active)
            .OrderByDescending(j => j.UpdatedAt)
            .Take(5);

        // Get default active journey for FAB
        var defaultActiveJourney = await _journeyService.GetUserDefaultActiveJourneyAsync(userId);

        // Get subscription limits and usage
        var canAddCar = await _carService.CanUserAddCarAsync(subTier, userId, cancellationToken);
        var canCreateJourney = await _journeyService.CanUserCreateJourneyAsync(userId);

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

        return Ok(new
        {
            user = new
            {
                user.Id,
                user.DisplayName,
                user.AvatarUrl,
                user.SubscriptionTier,
                user.EmailConfirmed
            },
            stats = journeyStats,
            limits = new
            {
                cars = new
                {
                    current = carCount,
                    max = maxCars == int.MaxValue ? "Unlimited" : maxCars.ToString(),
                    canAdd = canAddCar
                },
                journeys = new
                {
                    current = journeyStats.TotalJourneys,
                    max = maxJourneys == int.MaxValue ? "Unlimited" : maxJourneys.ToString(),
                    canCreate = canCreateJourney
                }
            },
            recentJourneys,
            defaultActiveJourney,
            quickActions = new
            {
                hasDefaultJourney = defaultActiveJourney != null,
                canCreatePost = defaultActiveJourney != null,
                canCreateJourney,
                canAddCar,
                needsCarRegistration = carCount == 0
            }
        });
    }

    /// <summary>
    /// Get activity feed for dashboard (subscribed journeys)
    /// </summary>
    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetActivityFeed(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var userId = User.GetUserId();

        // For now, return trending journeys as placeholder
        // In future, this would be journeys the user has subscribed to
        var feed = await _journeyService.GetTrendingJourneysAsync(userId);

        return Ok(feed.Skip(skip).Take(take));
    }
}
