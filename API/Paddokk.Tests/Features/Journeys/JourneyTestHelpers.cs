using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys;

internal static class JourneyTestHelpers
{
    internal static Journey BuildJourney(int id = 1, string userId = "user-1") => new()
    {
        Id = id,
        Title = "Test Journey",
        Category = JourneyCategory.BuildAndMods,
        Status = JourneyStatus.Active,
        PrincipalId = userId,
        UserCarId = 1,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        User = new ApplicationUser { Id = userId, DisplayName = "Test User" },
        UserCar = new UserCar { Id = 1, PrincipalId = userId },
        Posts = [],
        Subscriptions = [],
        Likes = []
    };

    internal static ApplicationUser BuildUser(string userId = "user-1") => new()
    {
        Id = userId,
        FirstName = "Test",
        LastName = "User",
        Username = $"test.user.{userId}",
        DisplayName = "Test User",
        SubscriptionTier = SubscriptionTier.Gold
    };
}
