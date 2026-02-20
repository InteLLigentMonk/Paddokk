using Paddokk.Core.Models.DTOs;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface ICarService
{
    // Car Database
    Task<IEnumerable<CarMakeDto>> GetCarMakesAsync();
    Task<IEnumerable<CarModelDto>> GetCarModelsByMakeAsync(int carMakeId);
    Task<IEnumerable<CarGenerationDto>> GetCarGenerationsByModelAsync(int carModelId);
    Task<CarMakeDto?> GetCarMakeByIdAsync(int carMakeId);
    Task<CarModelDto?> GetCarModelByIdAsync(int carModelId);
    Task<CarGenerationDto?> GetCarGenerationByIdAsync(int carGenerationId);

    // User Cars
    Task<IEnumerable<UserCarDto>> GetUserCarsAsync(string userId);
    Task<UserCarDto?> GetUserCarByIdAsync(string userId, int carId);
    Task<UserCarDto> CreateUserCarAsync(SubscriptionTier subTier, string userId, CreateUserCarRequest request);
    Task<UserCarDto?> UpdateUserCarAsync(string userId, int carId, UpdateUserCarRequest request);
    Task<bool> DeleteUserCarAsync(string userId, int carId);
    Task<int> GetUserCarCountAsync(string userId);
    Task<bool> CanUserAddCarAsync(SubscriptionTier subTier, string userId);
    Task<bool> ValidateCarCombinationAsync(CreateUserCarRequest request);
}
