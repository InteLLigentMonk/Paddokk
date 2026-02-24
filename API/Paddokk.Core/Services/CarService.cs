using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly ILogger<CarService> _logger;

    public CarService(ICarRepository carRepository, ILogger<CarService> logger)
    {
        _carRepository = carRepository;
        _logger = logger;
    }

    // Car Service Methods
    public async Task<IEnumerable<CarMakeDto>> GetCarMakesAsync(CancellationToken cancellationToken)
    {
        return (await _carRepository.GetCarMakesAsync(cancellationToken)).Select(m => new CarMakeDto
        {
            Id = m.Id,
            Name = m.Name,
            Country = m.Country,
            Group = m.Group,
            ModelCount = m.Models.Count
        });
    }

    public async Task<IEnumerable<CarModelDto>> GetCarModelsByMakeAsync(
        int carMakeId, CancellationToken cancellationToken)
    {
        return (await _carRepository.GetCarModelsByMakeAsync(carMakeId, cancellationToken))
            .Select(m => new CarModelDto
            {
                Id = m.Id,
                Name = m.Name,
                CarMakeId = m.CarMakeId,
                CarMakeName = m.CarMake.Name,
                GenerationCount = m.Generations.Count
            });
    }

    public async Task<IEnumerable<CarGenerationDto>> GetCarGenerationsByModelAsync(
        int carModelId, CancellationToken cancellationToken)
    {
        return (await _carRepository.GetCarGenerationsByModelAsync(carModelId, cancellationToken))
            .Select(g => new CarGenerationDto
            {
                Id = g.Id,
                Name = g.Name,
                StartYear = g.StartYear,
                EndYear = g.EndYear,
                CarModelId = g.CarModelId,
                CarModelName = g.CarModel.Name
            });
    }

    public async Task<CarMakeDto?> GetCarMakeByIdAsync(
        int carMakeId, CancellationToken cancellationToken)
    {
        var carMake = await _carRepository.GetCarMakeByIdAsync(carMakeId, cancellationToken);

        if (carMake == null)
            return null;

        return new CarMakeDto
            {
                Id = carMake.Id,
                Name = carMake.Name,
                Country = carMake.Country,
                Group = carMake.Group,
                ModelCount = carMake.Models.Count
            };
    }

    public async Task<CarModelDto?> GetCarModelByIdAsync(
        int carModelId, CancellationToken cancellationToken)
    {
        var carModel = await _carRepository.GetCarModelByIdAsync(carModelId, CancellationToken.None);

        if (carModel == null)
            return null;

        return new CarModelDto
            {
                Id = carModel.Id,
                Name = carModel.Name,
                CarMakeId = carModel.CarMakeId,
                CarMakeName = carModel.CarMake.Name,
                GenerationCount = carModel.Generations.Count
            };
    }

    public async Task<CarGenerationDto?> GetCarGenerationByIdAsync(
        int carGenerationId, CancellationToken cancellationToken)
    {
        var carGeneration = await _carRepository.GetCarGenerationByIdAsync(
            carGenerationId, cancellationToken);

        if (carGeneration == null)
            return null;

        return new CarGenerationDto
            {
                Id = carGeneration.Id,
                Name = carGeneration.Name,
                StartYear = carGeneration.StartYear,
                EndYear = carGeneration.EndYear,
                CarModelId = carGeneration.CarModelId,
                CarModelName = carGeneration.CarModel.Name
            };
    }

    // User Car Methods
    public async Task<IEnumerable<UserCarDto>> GetUserCarsAsync(
        string userId, CancellationToken cancellationToken)
    {
        return (await _carRepository.GetUserCarsAsync(userId, cancellationToken))
            .Select(c => new UserCarDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CarMakeId = c.CarMakeId,
                CarMakeName = c.CarMake.Name,
                CarModelId = c.CarModelId,
                CarModelName = c.CarModel.Name,
                CarGenerationId = c.CarGenerationId,
                CarGenerationName = c.CarGeneration?.Name,
                Year = c.Year,
                Nickname = c.Nickname,
                Color = c.Color,
                Description = c.Description,
                PrimaryImageUrl = c.PrimaryImageUrl,
                IsPrimary = c.IsPrimary,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                JourneyCount = c.Journeys.Count
            });
    }

    public async Task<UserCarDto?> GetUserCarByIdAsync(
        string userId, int carId, CancellationToken cancellationToken)
    {
        var userCar = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);

        if (userCar == null)
            return null;

        return new UserCarDto
            {
                Id = userCar.Id,
                UserId = userCar.UserId,
                CarMakeId = userCar.CarMakeId,
                CarMakeName = userCar.CarMake.Name,
                CarModelId = userCar.CarModelId,
                CarModelName = userCar.CarModel.Name,
                CarGenerationId = userCar.CarGenerationId,
                CarGenerationName = userCar.CarGeneration?.Name,
                Year = userCar.Year,
                Nickname = userCar.Nickname,
                Color = userCar.Color,
                Description = userCar.Description,
                PrimaryImageUrl = userCar.PrimaryImageUrl,
                IsPrimary = userCar.IsPrimary,
                CreatedAt = userCar.CreatedAt,
                UpdatedAt = userCar.UpdatedAt,
                JourneyCount = userCar.Journeys.Count
            };
    }

    public async Task<UserCarDto> CreateUserCarAsync(
        SubscriptionTier subTier,
        string userId,
        CreateUserCarRequest request,
        CancellationToken cancellationToken)
    {
        // Validate subscription limits
        var carLimit = await CanUserAddCarAsync(subTier, userId, cancellationToken);
        if (!carLimit.CanAdd)
            throw new InvalidOperationException("Car limit reached for current subscription tier");

        // Validate car combination
        if (!await ValidateCarCombinationAsync(request, cancellationToken))
            throw new ArgumentException("Invalid car make/model/generation combination");

        // If this is the user's first car, make it primary
        var userCarCount = await GetUserCarCountAsync(userId, cancellationToken);
        var isPrimary = request.IsPrimary || userCarCount == 0;

        // If setting as primary, unset other primary cars
        if (isPrimary)
        {
            await _carRepository.UnsetPrimaryCar(userId, cancellationToken);
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

        await _carRepository.CreateUserCarAsync(userCar, cancellationToken);

        _logger.LogInformation("User {UserId} created car {CarId}: {Make} {Model} {Year}",
            userId, userCar.Id, request.CarMakeId, request.CarModelId, request.Year);

        return await GetUserCarByIdAsync(userId, userCar.Id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to retrieve created car");
    }

    public async Task<UserCarDto?> UpdateUserCarAsync(string userId, int carId, UpdateUserCarRequest request, CancellationToken cancellationToken)
    {
        var userCar = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);

        if (userCar == null)
            throw new KeyNotFoundException($"Car {carId} not found");

        // If setting as primary, unset other primary cars
        if (request.IsPrimary == true)
        {
            await _carRepository.UnsetPrimaryCar(userId, cancellationToken);
        }

        // Update fields
        if (request.Nickname != null) userCar.Nickname = request.Nickname;
        if (request.Color != null) userCar.Color = request.Color;
        if (request.Description != null) userCar.Description = request.Description;
        if (request.IsPrimary.HasValue) userCar.IsPrimary = request.IsPrimary.Value;

        userCar.UpdatedAt = DateTime.UtcNow;

        await _carRepository.UpdateUserCarAsync(userCar, cancellationToken);

        _logger.LogInformation("User {UserId} updated car {CarId}", userId, carId);

        return await GetUserCarByIdAsync(userId, carId, cancellationToken);
    }

    public async Task<bool> DeleteUserCarAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        var userCar = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);

        if (userCar == null)
            return false;

        // Check if car has active journeys
        if (userCar.Journeys.Any(j => j.Status == JourneyStatus.Active))
            throw new InvalidOperationException("Cannot delete car with active journeys");

        await _carRepository.DeleteUserCarAsync(userId, carId, cancellationToken);

        _logger.LogInformation("User {UserId} deleted car {CarId}", userId, carId);

        return true;
    }

    public async Task<int> GetUserCarCountAsync(string userId, CancellationToken cancellationToken)
    {
        return await _carRepository.GetUserCarCountAsync(userId, cancellationToken);
    }

    public async Task<CarLimitDto> CanUserAddCarAsync(SubscriptionTier subTier, string userId, CancellationToken cancellationToken)
    {
        var currentCarCount = await GetUserCarCountAsync(userId, cancellationToken);
        var maxCars = subTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };
        return new CarLimitDto
        {
            CanAdd = currentCarCount < maxCars,
            CurrentCount = currentCarCount,
            MaxAllowed = maxCars == int.MaxValue ? "Unlimited" : maxCars.ToString(),
            SubscriptionTier = subTier.ToString()
        };
    }

    public async Task<bool> ValidateCarCombinationAsync(CreateUserCarRequest request, CancellationToken cancellationToken)
    {
        // Validate make exists
        var make = await _carRepository.GetCarMakeByIdAsync(request.CarMakeId, cancellationToken);
        if (make == null) return false;

        // Validate model belongs to make
        var model = await _carRepository.GetCarModelByIdAsync(request.CarModelId, cancellationToken);
        if (model == null || model.CarMakeId != request.CarMakeId) return false;

        // Validate generation belongs to model (if specified)
        if (request.CarGenerationId.HasValue)
        {
            var generation = await _carRepository.GetCarGenerationByIdAsync(request.CarGenerationId.Value, cancellationToken);
            if (generation == null || generation.CarModelId != request.CarModelId) return false;

            // Validate year falls within generation range
            if (request.Year < generation.StartYear ||
                generation.EndYear.HasValue && request.Year > generation.EndYear)
                return false;
        }

        return true;
    }

    public async Task<bool> UserOwnsCarAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        return await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken) is not null;
    }

    public async Task UpdatePrimaryImageUrlAsync(int carId, string? imageUrl, CancellationToken cancellationToken)
    {
        await _carRepository.UpdatePrimaryImageUrlAsync(carId, imageUrl, cancellationToken);
    }
}
