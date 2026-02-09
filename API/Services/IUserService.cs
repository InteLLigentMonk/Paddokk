using API.Models.DTOs;

namespace API.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserRequest request);
}
