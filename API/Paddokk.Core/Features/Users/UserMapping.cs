using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Users;

internal static class UserMapping
{
    internal static UserDto ToDto(ApplicationUser user)
    {
        var maxCars = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            EmailConfirmed = false, // Better Auth manages email verification separately
            SubscriptionTier = user.SubscriptionTier,
            SubscriptionExpiresAt = user.SubscriptionExpiresAt,
            DefaultActiveJourneyId = user.DefaultActiveJourneyId,
            CarCount = user.Cars?.Count ?? 0,
            JourneyCount = user.Journeys?.Count ?? 0,
            MaxCars = maxCars
        };
    }
}
