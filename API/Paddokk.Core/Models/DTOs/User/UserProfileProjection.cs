using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.User;

/// <summary>
/// A loaded user plus its profile counts, all resolved in a single SQL query
/// (counts are correlated subqueries, not loaded rows).
/// </summary>
public sealed record UserProfileProjection(
    ApplicationUser User,
    int CarCount,
    int JourneyCount,
    int FollowerCount,
    int FollowingCount,
    bool IsFollowedByMe);
