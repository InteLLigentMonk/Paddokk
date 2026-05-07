using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class CarRepository : ICarRepository
{
    private readonly PaddokkDbContext _db;

    public CarRepository(PaddokkDbContext db)
    {
        _db = db;
    }

    public async Task<List<CarMake>> GetCarMakesAsync(CancellationToken cancellationToken)
    {
        return await _db.CarMakes
            .Include(m => m.Models)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CarModel>> GetCarModelsByMakeAsync(int carMakeId, CancellationToken cancellationToken)
    {
        return await _db.CarModels
            .Include(m => m.CarMake)
            .Include(m => m.Generations)
            .Where(m => m.CarMakeId == carMakeId)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CarGeneration>> GetCarGenerationsByModelAsync(int carModelId, CancellationToken cancellationToken)
    {
        return await _db.CarGenerations
            .Include(g => g.CarModel)
            .Where(g => g.CarModelId == carModelId)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<CarMake?> GetCarMakeByIdAsync(int carMakeId, CancellationToken cancellationToken)
    {
        return await _db.CarMakes
            .Include(m => m.Models)
            .FirstOrDefaultAsync(m => m.Id == carMakeId, cancellationToken);
    }

    public async Task<CarModel?> GetCarModelByIdAsync(int carModelId, CancellationToken cancellationToken)
    {
        return await _db.CarModels
            .Include(m => m.CarMake)
            .Include(m => m.Generations)
            .FirstOrDefaultAsync(m => m.Id == carModelId, cancellationToken);
    }

    public async Task<CarGeneration?> GetCarGenerationByIdAsync(int carGenerationId, CancellationToken cancellationToken)
    {
        return await _db.CarGenerations
            .Include(g => g.CarModel)
            .FirstOrDefaultAsync(g => g.Id == carGenerationId, cancellationToken);
    }

    public async Task<List<UserCar>> GetUserCarsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Include(c => c.Likes)
            .Include(c => c.Subscriptions)
            .Where(c => c.PrincipalId == userId && c.IsActive)
            .OrderByDescending(c => c.IsPrimary)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserCar?> GetUserCarByIdAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        return await _db.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Include(c => c.Likes)
            .Include(c => c.Subscriptions)
            .Where(c => c.PrincipalId == userId && c.Id == carId && c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CreateUserCarAsync(UserCar userCar, CancellationToken cancellationToken)
    {
        _db.UserCars.Add(userCar);
        await _db.SaveChangesAsync(cancellationToken);
        return userCar.Id;
    }
    public async Task UpdateUserCarAsync(UserCar userCar, CancellationToken cancellationToken)
    {
        _db.UserCars.Update(userCar);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserCarAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        await _db.UserCars
            .Where(c => c.Id == carId && c.PrincipalId == userId)
            .ExecuteUpdateAsync(c => c
                .SetProperty(p => p.IsActive, false)
                .SetProperty(p => p.UpdatedAt, DateTime.UtcNow), cancellationToken);
    }

    public async Task UnsetPrimaryCar(string userId, CancellationToken cancellationToken)
    {
        await _db.UserCars
                .Where(c => c.PrincipalId == userId && c.IsPrimary)
                .ExecuteUpdateAsync(c => c
                .SetProperty(p => p.IsPrimary, false), cancellationToken);
    }

    public Task<int> GetUserCarCountAsync(string userId, CancellationToken cancellationToken)
    {
        return _db.UserCars
            .Where(c => c.PrincipalId == userId && c.IsActive)
            .CountAsync(cancellationToken);
    }

    public async Task UpdatePrimaryImageUrlAsync(int carId, string? imageUrl, CancellationToken cancellationToken)
    {
        await _db.UserCars
            .Where(c => c.Id == carId)
            .ExecuteUpdateAsync(c => c
                .SetProperty(p => p.PrimaryImageUrl, imageUrl)
                .SetProperty(p => p.UpdatedAt, DateTime.UtcNow), cancellationToken);
    }

    public async Task<UserCar?> GetCarByIdAsync(int carId, CancellationToken cancellationToken)
    {
        return await _db.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Include(c => c.Likes)
            .Include(c => c.Subscriptions)
            .FirstOrDefaultAsync(c => c.Id == carId && c.IsActive, cancellationToken);
    }

    public async Task<UserCarLike?> GetCarLikeAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        return await _db.UserCarLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.UserCarId == carId, cancellationToken);
    }

    public async Task CreateCarLikeAsync(UserCarLike like, CancellationToken cancellationToken)
    {
        _db.UserCarLikes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCarLikeAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        await _db.UserCarLikes
            .Where(l => l.UserId == userId && l.UserCarId == carId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<UserCarSubscription?> GetCarSubscriptionAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        return await _db.UserCarSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.UserCarId == carId, cancellationToken);
    }

    public async Task CreateCarSubscriptionAsync(UserCarSubscription subscription, CancellationToken cancellationToken)
    {
        _db.UserCarSubscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCarSubscriptionAsync(UserCarSubscription subscription, CancellationToken cancellationToken)
    {
        _db.UserCarSubscriptions.Update(subscription);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<UserCar>> SearchCarsAsync(string query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var pattern = $"%{query.Trim()}%";

        return await _db.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Journeys)
            .Include(c => c.Likes)
            .Include(c => c.Subscriptions)
            .Where(c => c.IsActive &&
                c.SearchText != null &&
                EF.Functions.ILike(c.SearchText, pattern))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
