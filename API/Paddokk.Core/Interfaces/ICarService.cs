using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface ICarService
{
    // Car Services
    Task<IEnumerable<CarMakeDto>> GetCarMakesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<CarModelDto>> GetCarModelsByMakeAsync(int carMakeId, CancellationToken cancellationToken);
    Task<IEnumerable<CarGenerationDto>> GetCarGenerationsByModelAsync(int carModelId, CancellationToken cancellationToken);
    Task<CarMakeDto> GetCarMakeByIdAsync(int carMakeId, CancellationToken cancellationToken);
    Task<CarModelDto> GetCarModelByIdAsync(int carModelId, CancellationToken cancellationToken);
    Task<CarGenerationDto> GetCarGenerationByIdAsync(int carGenerationId, CancellationToken cancellationToken);

    // User Car Services
    Task<IEnumerable<UserCarDto>> GetUserCarsAsync(string userId, CancellationToken cancellationToken);
    Task<UserCarDto> GetUserCarByIdAsync(string userId, int carId, CancellationToken cancellationToken);
    Task<UserCarDto> CreateUserCarAsync(SubscriptionTier subTier, string userId, CreateUserCarRequest request, CancellationToken cancellationToken);
    Task<UserCarDto> UpdateUserCarAsync(string userId, int carId, UpdateUserCarRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteUserCarAsync(string userId, int carId, CancellationToken cancellationToken);
    Task<int> GetUserCarCountAsync(string userId, CancellationToken cancellationToken);
    Task<CarLimitDto> CanUserAddCarAsync(SubscriptionTier subTier, string userId, CancellationToken cancellationToken);
    Task<bool> ValidateCarCombinationAsync(CreateUserCarRequest request, CancellationToken cancellationToken);
    Task UserOwnsCarAsync(string userId, int carId, CancellationToken cancellationToken);
    Task UpdatePrimaryImageUrlAsync(int carId, string? imageUrl, CancellationToken cancellationToken);
}
