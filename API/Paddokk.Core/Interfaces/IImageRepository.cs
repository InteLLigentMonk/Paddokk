using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IImageRepository
{
    Task<int> GetImageCountByContextAsync(string context, int contextId, CancellationToken cancellationToken);

    Task<IEnumerable<UserCarImage>> GetCarImagesAsync(int carId, CancellationToken cancellationToken);

    Task<UserCarImage?> GetCarImageByIdAsync(int carImageId, CancellationToken cancellationToken);

    Task<UserCarImage?> GetCarImageByIdAsync(int carImageId, string userId, CancellationToken cancellationToken);

    Task AddCarImageAsync(UserCarImage carImage, CancellationToken cancellationToken);

    Task UpdateCarImageAsync(UserCarImage carImage, CancellationToken cancellationToken);

    Task DeleteCarImageAsync(int carImageId, CancellationToken cancellationToken);

    Task SetPrimaryImageAsync(int carId, int imageId, CancellationToken cancellationToken);

    Task<UserCarImage?> GetNextPrimaryImageAsync(int carId, int excludeImageId, CancellationToken cancellationToken);
}
