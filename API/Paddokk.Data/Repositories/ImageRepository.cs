using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class ImageRepository(PaddokkDbContext db) : IImageRepository
{
    private readonly PaddokkDbContext _db = db;

    public Task<int> GetImageCountByContextAsync(string context, int contextId, CancellationToken cancellationToken)
    {
        return _db.UserCarImages
            .CountAsync(i => i.UserCarId == contextId, cancellationToken);
    }

 public async Task<IEnumerable<UserCarImage>> GetCarImagesAsync(int carId, CancellationToken cancellationToken)
    {
        return await _db.UserCarImages
            .AsNoTracking()
            .Where(i => i.UserCarId == carId)
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserCarImage?> GetCarImageByIdAsync(int carImageId, CancellationToken cancellationToken)
    {
        return await _db.UserCarImages
            .Include(i => i.UserCar)
            .FirstOrDefaultAsync(i => i.Id == carImageId, cancellationToken);
    }

    public async Task<UserCarImage?> GetCarImageByIdAsync(int carImageId, string userId, CancellationToken cancellationToken)
    {
        return await _db.UserCarImages
            .Include(i => i.UserCar)
            .FirstOrDefaultAsync(i => i.Id == carImageId && i.UserCar.PrincipalId == userId, cancellationToken);
    }

    public async Task AddCarImageAsync(UserCarImage carImage, CancellationToken cancellationToken)
    {
        _db.UserCarImages.Add(carImage);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCarImageAsync(UserCarImage carImage, CancellationToken cancellationToken)
    {
        _db.UserCarImages.Update(carImage);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCarImageAsync(int carImageId, CancellationToken cancellationToken)
    {
        await _db.UserCarImages
            .Where(i => i.Id == carImageId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SetPrimaryImageAsync(int carId, int imageId, CancellationToken cancellationToken)
    {
        // Unset all primary images for this car
        await _db.UserCarImages
            .Where(i => i.UserCarId == carId && i.IsPrimary)
            .ExecuteUpdateAsync(i => i.SetProperty(p => p.IsPrimary, false), cancellationToken);

        // Set the new primary
        await _db.UserCarImages
            .Where(i => i.Id == imageId)
            .ExecuteUpdateAsync(i => i.SetProperty(p => p.IsPrimary, true), cancellationToken);
  }

    public async Task<UserCarImage?> GetNextPrimaryImageAsync(int carId, int excludeImageId, CancellationToken cancellationToken)
    {
        return await _db.UserCarImages
            .Where(i => i.UserCarId == carId && i.Id != excludeImageId)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
