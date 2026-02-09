using System.Security.Claims;
using API.Data;
using API.Extensions;
using API.Models.DTOs;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class CarService : ICarService
{
    private readonly PaddokkDbContext _context;
    private readonly ILogger<CarService> _logger;

    public CarService(PaddokkDbContext context, ILogger<CarService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Car Database Methods
    public async Task<IEnumerable<CarMakeDto>> GetCarMakesAsync()
    {
        return await _context.CarMakes
            .Include(m => m.Models)
            .Select(m => new CarMakeDto
            {
                Id = m.Id,
                Name = m.Name,
                Country = m.Country,
                Group = m.Group,
                ModelCount = m.Models.Count
            })
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CarModelDto>> GetCarModelsByMakeAsync(int carMakeId)
    {
        return await _context.CarModels
            .Include(m => m.CarMake)
            .Include(m => m.Generations)
            .Where(m => m.CarMakeId == carMakeId)
            .Select(m => new CarModelDto
            {
                Id = m.Id,
                Name = m.Name,
                CarMakeId = m.CarMakeId,
                CarMakeName = m.CarMake.Name,
                GenerationCount = m.Generations.Count
            })
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CarGenerationDto>> GetCarGenerationsByModelAsync(int carModelId)
    {
        return await _context.CarGenerations
            .Include(g => g.CarModel)
            .Where(g => g.CarModelId == carModelId)
            .Select(g => new CarGenerationDto
            {
                Id = g.Id,
                Name = g.Name,
                StartYear = g.StartYear,
                EndYear = g.EndYear,
                CarModelId = g.CarModelId,
                CarModelName = g.CarModel.Name
            })
            .OrderByDescending(g => g.StartYear)
            .ToListAsync();
    }

    public async Task<CarMakeDto?> GetCarMakeByIdAsync(int carMakeId)
    {
        return await _context.CarMakes
            .Include(m => m.Models)
            .Where(m => m.Id == carMakeId)
            .Select(m => new CarMakeDto
            {
                Id = m.Id,
                Name = m.Name,
                Country = m.Country,
                Group = m.Group,
                ModelCount = m.Models.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CarModelDto?> GetCarModelByIdAsync(int carModelId)
    {
        return await _context.CarModels
            .Include(m => m.CarMake)
            .Include(m => m.Generations)
            .Where(m => m.Id == carModelId)
            .Select(m => new CarModelDto
            {
                Id = m.Id,
                Name = m.Name,
                CarMakeId = m.CarMakeId,
                CarMakeName = m.CarMake.Name,
                GenerationCount = m.Generations.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CarGenerationDto?> GetCarGenerationByIdAsync(int carGenerationId)
    {
        return await _context.CarGenerations
            .Include(g => g.CarModel)
            .Where(g => g.Id == carGenerationId)
            .Select(g => new CarGenerationDto
            {
                Id = g.Id,
                Name = g.Name,
                StartYear = g.StartYear,
                EndYear = g.EndYear,
                CarModelId = g.CarModelId,
                CarModelName = g.CarModel.Name
            })
            .FirstOrDefaultAsync();
    }

    // User Car Methods
    public async Task<IEnumerable<UserCarDto>> GetUserCarsAsync(string userId)
    {
        return await _context.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Where(c => c.UserId == userId && c.IsActive)
            .Select(c => new UserCarDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CarMakeId = c.CarMakeId,
                CarMakeName = c.CarMake.Name,
                CarModelId = c.CarModelId,
                CarModelName = c.CarModel.Name,
                CarGenerationId = c.CarGenerationId,
                CarGenerationName = c.CarGeneration != null ? c.CarGeneration.Name : null,
                Year = c.Year,
                Nickname = c.Nickname,
                Color = c.Color,
                Description = c.Description,
                PrimaryImageUrl = c.PrimaryImageUrl,
                IsPrimary = c.IsPrimary,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                JourneyCount = c.Journeys.Count
            })
            .OrderByDescending(c => c.IsPrimary)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserCarDto?> GetUserCarByIdAsync(string userId, int carId)
    {
        return await _context.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Where(c => c.UserId == userId && c.Id == carId && c.IsActive)
            .Select(c => new UserCarDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CarMakeId = c.CarMakeId,
                CarMakeName = c.CarMake.Name,
                CarModelId = c.CarModelId,
                CarModelName = c.CarModel.Name,
                CarGenerationId = c.CarGenerationId,
                CarGenerationName = c.CarGeneration != null ? c.CarGeneration.Name : null,
                Year = c.Year,
                Nickname = c.Nickname,
                Color = c.Color,
                Description = c.Description,
                PrimaryImageUrl = c.PrimaryImageUrl,
                IsPrimary = c.IsPrimary,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                JourneyCount = c.Journeys.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserCarDto> CreateUserCarAsync(SubscriptionTier subTier, string userId, CreateUserCarRequest request)
    {
        // Validate subscription limits
        if (!await CanUserAddCarAsync(subTier, userId))
            throw new InvalidOperationException("Car limit reached for current subscription tier");

        // Validate car combination
        if (!await ValidateCarCombinationAsync(request))
            throw new ArgumentException("Invalid car make/model/generation combination");

        // If this is the user's first car, make it primary
        var userCarCount = await GetUserCarCountAsync(userId);
        var isPrimary = request.IsPrimary || userCarCount == 0;

        // If setting as primary, unset other primary cars
        if (isPrimary)
        {
            await _context.UserCars
                .Where(c => c.UserId == userId && c.IsPrimary)
                .ExecuteUpdateAsync(c => c.SetProperty(p => p.IsPrimary, false));
        }

        var userCar = new UserCar
        {
            UserId = userId,
            CarMakeId = request.CarMakeId,
            CarModelId = request.CarModelId,
            CarGenerationId = request.CarGenerationId,
            Year = request.Year,
            Nickname = request.Nickname,
            Color = request.Color,
            Description = request.Description,
            IsPrimary = isPrimary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserCars.Add(userCar);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} created car {CarId}: {Make} {Model} {Year}",
            userId, userCar.Id, request.CarMakeId, request.CarModelId, request.Year);

        return await GetUserCarByIdAsync(userId, userCar.Id)
            ?? throw new InvalidOperationException("Failed to retrieve created car");
    }

    public async Task<UserCarDto?> UpdateUserCarAsync(string userId, int carId, UpdateUserCarRequest request)
    {
        var userCar = await _context.UserCars
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == carId && c.IsActive);

        if (userCar == null)
            return null;

        // If setting as primary, unset other primary cars
        if (request.IsPrimary == true)
        {
            await _context.UserCars
                .Where(c => c.UserId == userId && c.IsPrimary && c.Id != carId)
                .ExecuteUpdateAsync(c => c.SetProperty(p => p.IsPrimary, false));
        }

        // Update fields
        if (request.Nickname != null) userCar.Nickname = request.Nickname;
        if (request.Color != null) userCar.Color = request.Color;
        if (request.Description != null) userCar.Description = request.Description;
        if (request.IsPrimary.HasValue) userCar.IsPrimary = request.IsPrimary.Value;

        userCar.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated car {CarId}", userId, carId);

        return await GetUserCarByIdAsync(userId, carId);
    }

    public async Task<bool> DeleteUserCarAsync(string userId, int carId)
    {
        var userCar = await _context.UserCars
            .Include(c => c.Journeys)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == carId && c.IsActive);

        if (userCar == null)
            return false;

        // Check if car has active journeys
        if (userCar.Journeys.Any(j => j.Status == JourneyStatus.Active))
            throw new InvalidOperationException("Cannot delete car with active journeys");

        // Soft delete
        userCar.IsActive = false;
        userCar.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted car {CarId}", userId, carId);

        return true;
    }

    public async Task<int> GetUserCarCountAsync(string userId)
    {
        return await _context.UserCars
            .CountAsync(c => c.UserId == userId && c.IsActive);
    }

    public async Task<bool> CanUserAddCarAsync(SubscriptionTier subTier, string userId)
    {
        var currentCarCount = await GetUserCarCountAsync(userId);
        var maxCars = subTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };
        return currentCarCount < maxCars;
    }

    public async Task<bool> ValidateCarCombinationAsync(CreateUserCarRequest request)
    {
        // Validate make exists
        var make = await _context.CarMakes.FindAsync(request.CarMakeId);
        if (make == null) return false;

        // Validate model belongs to make
        var model = await _context.CarModels
            .FirstOrDefaultAsync(m => m.Id == request.CarModelId && m.CarMakeId == request.CarMakeId);
        if (model == null) return false;

        // Validate generation belongs to model (if specified)
        if (request.CarGenerationId.HasValue)
        {
            var generation = await _context.CarGenerations
                .FirstOrDefaultAsync(g => g.Id == request.CarGenerationId && g.CarModelId == request.CarModelId);
            if (generation == null) return false;

            // Validate year falls within generation range
            if (request.Year < generation.StartYear ||
                (generation.EndYear.HasValue && request.Year > generation.EndYear))
                return false;
        }

        return true;
    }
}
