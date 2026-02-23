using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IImageRepository
{
    Task<int> GetImageCountByContextAsync(string context, int contextId, CancellationToken cancellationToken);

    Task<IEnumerable<UserCarImage>> GetCarImagesAsync(int carId, CancellationToken cancellationToken);
}
