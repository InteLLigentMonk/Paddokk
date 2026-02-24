using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken);
}
