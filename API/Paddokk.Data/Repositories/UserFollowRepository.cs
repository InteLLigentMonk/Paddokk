using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class UserFollowRepository(PaddokkDbContext db) : IUserFollowRepository
{
    private readonly PaddokkDbContext _db = db;

    public async Task<UserFollow?> GetFollowAsync(string followerId, string followedId, CancellationToken cancellationToken)
    {
        return await _db.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId, cancellationToken);
    }

    public async Task CreateFollowAsync(UserFollow follow, CancellationToken cancellationToken)
    {
        _db.UserFollows.Add(follow);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateFollowAsync(UserFollow follow, CancellationToken cancellationToken)
    {
        _db.UserFollows.Update(follow);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<UserProfileProjection> Items, int TotalCount)> GetFollowersAsync(
        string userId, string? actorUserId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        var query = _db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowedId == userId && f.IsActive)
            .OrderByDescending(f => f.CreatedAt);

        var total = await query.CountAsync(cancellationToken);

        // Project the follower side. Counts become correlated subqueries; isFollowedByMe is
        // evaluated per row so a viewer can follow back without an extra round-trip.
        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(f => new UserProfileProjection(
                f.Follower,
                f.Follower.Cars.Count,
                f.Follower.Journeys.Count,
                f.Follower.Followers.Count(x => x.IsActive),
                f.Follower.Following.Count(x => x.IsActive),
                actorUserId != null && f.Follower.Followers.Any(x => x.FollowerId == actorUserId && x.IsActive)))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<(IReadOnlyList<UserProfileProjection> Items, int TotalCount)> GetFollowingAsync(
        string userId, string? actorUserId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        var query = _db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId && f.IsActive)
            .OrderByDescending(f => f.CreatedAt);

        var total = await query.CountAsync(cancellationToken);

        // Project the followed side (same shape as GetFollowersAsync).
        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .Select(f => new UserProfileProjection(
                f.Followed,
                f.Followed.Cars.Count,
                f.Followed.Journeys.Count,
                f.Followed.Followers.Count(x => x.IsActive),
                f.Followed.Following.Count(x => x.IsActive),
                actorUserId != null && f.Followed.Followers.Any(x => x.FollowerId == actorUserId && x.IsActive)))
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
