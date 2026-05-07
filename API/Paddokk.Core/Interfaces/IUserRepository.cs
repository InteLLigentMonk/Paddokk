using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken);

    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);

    Task CreateAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task SoftDeleteAsync(string userId, CancellationToken cancellationToken);
}
