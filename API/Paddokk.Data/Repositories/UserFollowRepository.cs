using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
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
}
