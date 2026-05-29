using Paddokk.Core.Models.DTOs.User;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken);

    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    // Profile reads: user + counts (cars, journeys, followers, following) and the actor-scoped
    // isFollowedByMe, all projected in one SQL query. actorUserId is null for anonymous callers.
    Task<UserProfileProjection?> GetProfileByIdAsync(string userId, string? actorUserId, CancellationToken cancellationToken);

    Task<UserProfileProjection?> GetProfileByEmailAsync(string email, string? actorUserId, CancellationToken cancellationToken);

    Task<UserProfileProjection?> GetProfileByUsernameAsync(string username, string? actorUserId, CancellationToken cancellationToken);

    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);

    Task<bool> UsernameIsReservedAsync(string username, CancellationToken cancellationToken);

    Task CreateAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task SoftDeleteAsync(string userId, CancellationToken cancellationToken);

    Task ReserveUsernameAsync(ReservedUsername reservation, CancellationToken cancellationToken);
}
