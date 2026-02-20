using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs;
using Paddokk.Core.Models.Entities;

namespace API.Services;

public class UserService : IUserService
{
    private readonly PaddokkDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(
        PaddokkDbContext context,
        ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.Cars)
            .Include(u => u.Journeys)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.Cars)
            .Include(u => u.Journeys)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

        return user != null ? MapToUserDto(user) : null;
    }


    public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null || user.IsDeleted)
            return null;

        if (!string.IsNullOrEmpty(request.DisplayName))
            user.DisplayName = request.DisplayName;
        
        if (request.Bio != null)
            user.Bio = request.Bio;
        
        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user {UserId}", userId);
            return null;
        }

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
            return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {UserId}", userId);
            return false;
        }
    }

    private static UserDto MapToUserDto(ApplicationUser user)
    {
        var maxCars = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            SubscriptionTier = user.SubscriptionTier,
            SubscriptionExpiresAt = user.SubscriptionExpiresAt,
            DefaultActiveJourneyId = user.DefaultActiveJourneyId,
            CarCount = user.Cars?.Count ?? 0,
            JourneyCount = user.Journeys?.Count ?? 0,
            MaxCars = maxCars
        };
    }
}
