using Microsoft.AspNetCore.Http;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Interfaces;

public interface IImageService
{
    // Image Upload & Processing
    Task<ImageUploadDto> UploadImageAsync(IFormFile file, ImageContext context, CancellationToken cancellationToken, int? contextId = null, string? caption = null);
    Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken);
    Task<ImageLimitsDto> GetImageLimitsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> CanUserUploadImageAsync(string userId, ImageContext context, CancellationToken cancellationToken, int? contextId = null);
    Task<CanUploadImageResponse> GetUploadStatusAsync(string userId, int carId, CancellationToken cancellationToken);

    // Car Images
    Task<CarImagesResponse> GetCarImagesAsync(int carId, CancellationToken cancellationToken);
    Task<CarImageDto> GetCarImageByIdAsync(int carImageId, int carId, CancellationToken cancellationToken);
    Task<CarImageDto> AddCarImageAsync(string userId, int carId, IFormFile file, CancellationToken cancellationToken, string? caption = null);
    Task<CarImageDto> UpdateCarImageAsync(string userId, int carImageId, UpdateCarImageRequest request, CancellationToken cancellationToken);
    Task DeleteCarImageAsync(string userId, int carId, int carImageId, CancellationToken cancellationToken);
    Task SetCarPrimaryImageAsync(string userId, int carId, int carImageId, CancellationToken cancellationToken);

    // Journey Post Images (integrated with existing post creation)
    Task ValidatePostImagesAsync(string userId, List<CreateJourneyPostImageRequest> images, CancellationToken cancellationToken);

    // Image URL Generation
    Task<string> GenerateSecureUploadUrlAsync(string fileName, ImageContext context, CancellationToken cancellationToken);
    Task<string> GetOptimizedImageUrlAsync(string originalUrl, CancellationToken cancellationToken, int? width = null, int? height = null);
}
