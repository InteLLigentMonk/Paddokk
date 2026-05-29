using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IUserFollowRepository
{
    Task<UserFollow?> GetFollowAsync(string followerId, string followedId, CancellationToken cancellationToken);

    Task CreateFollowAsync(UserFollow follow, CancellationToken cancellationToken);

    Task UpdateFollowAsync(UserFollow follow, CancellationToken cancellationToken);

    // Active followers of userId (newest first), each projected with profile counts and
    // isFollowedByMe relative to actorUserId (null for anonymous callers).
    Task<(IReadOnlyList<UserProfileProjection> Items, int TotalCount)> GetFollowersAsync(
        string userId, string? actorUserId, int page, int pageSize, CancellationToken cancellationToken);

    // Users that userId actively follows (newest first), projected like GetFollowersAsync.
    Task<(IReadOnlyList<UserProfileProjection> Items, int TotalCount)> GetFollowingAsync(
        string userId, string? actorUserId, int page, int pageSize, CancellationToken cancellationToken);
}
