using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserRequest request);
}
