using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Users;

internal static class UserMapping
{
    // Profile reads: counts (cars, journeys, followers, following) and isFollowedByMe come
    // from a single projected SQL query.
    internal static UserDto ToDto(UserProfileProjection projection) =>
        ToDto(
            projection.User,
            projection.CarCount,
            projection.JourneyCount,
            projection.FollowerCount,
            projection.FollowingCount,
            projection.IsFollowedByMe);

    // Write paths (update profile / change username) return the actor's own profile; follow
    // counts default to 0/false since the client invalidates and refetches the projected reads.
    internal static UserDto ToDto(ApplicationUser user) =>
        ToDto(user, user.Cars?.Count ?? 0, user.Journeys?.Count ?? 0, 0, 0, false);

    private static UserDto ToDto(
        ApplicationUser user,
        int carCount,
        int journeyCount,
        int followerCount,
        int followingCount,
        bool isFollowedByMe)
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
            CarCount = carCount,
            JourneyCount = journeyCount,
            MaxCars = maxCars,
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            IsFollowedByMe = isFollowedByMe
        };
    }
}
