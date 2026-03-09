using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface ICarRepository
{
    Task<List<CarMake>> GetCarMakesAsync(CancellationToken cancellationToken);

    Task<List<CarModel>> GetCarModelsByMakeAsync(int carMakeId, CancellationToken cancellationToken);

    Task<List<CarGeneration>> GetCarGenerationsByModelAsync(int carModelId, CancellationToken cancellationToken);

    Task<CarMake?> GetCarMakeByIdAsync(int carMakeId, CancellationToken cancellationToken);

    Task<CarModel?> GetCarModelByIdAsync(int carModelId, CancellationToken cancellationToken);

    Task<CarGeneration?> GetCarGenerationByIdAsync(int carGenerationId, CancellationToken cancellationToken);

    Task<List<UserCar>> GetUserCarsAsync(string userId, CancellationToken cancellationToken);

    Task<UserCar?> GetUserCarByIdAsync(string userId, int carId, CancellationToken cancellationToken);

    Task<int> CreateUserCarAsync(UserCar userCar, CancellationToken cancellationToken);

    Task UnsetPrimaryCar(string userId, CancellationToken cancellationToken);

    Task UpdateUserCarAsync(UserCar userCar, CancellationToken cancellationToken);

    Task DeleteUserCarAsync(string userId, int carId, CancellationToken cancellationToken);

    Task<int> GetUserCarCountAsync(string userId, CancellationToken cancellationToken);

    Task UpdatePrimaryImageUrlAsync(int carId, string? imageUrl, CancellationToken cancellationToken);

    Task<UserCar?> GetCarByIdAsync(int carId, CancellationToken cancellationToken);

    Task<UserCarLike?> GetCarLikeAsync(string userId, int carId, CancellationToken cancellationToken);
    Task CreateCarLikeAsync(UserCarLike like, CancellationToken cancellationToken);
    Task DeleteCarLikeAsync(string userId, int carId, CancellationToken cancellationToken);

    Task<UserCarSubscription?> GetCarSubscriptionAsync(string userId, int carId, CancellationToken cancellationToken);
    Task CreateCarSubscriptionAsync(UserCarSubscription subscription, CancellationToken cancellationToken);
    Task UpdateCarSubscriptionAsync(UserCarSubscription subscription, CancellationToken cancellationToken);
}
