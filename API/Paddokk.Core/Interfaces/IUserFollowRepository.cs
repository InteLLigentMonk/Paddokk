using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IUserFollowRepository
{
    Task<UserFollow?> GetFollowAsync(string followerId, string followedId, CancellationToken cancellationToken);

    Task CreateFollowAsync(UserFollow follow, CancellationToken cancellationToken);

    Task UpdateFollowAsync(UserFollow follow, CancellationToken cancellationToken);
}
