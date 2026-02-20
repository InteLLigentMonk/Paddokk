using Microsoft.AspNetCore.Http;
using Paddokk.Core.Models.DTOs;

namespace Paddokk.Core.Interfaces;

public interface IImageService
{
    // Image Upload & Processing
    Task<ImageUploadDto> UploadImageAsync(IFormFile file, ImageContext context, int? contextId = null, string? caption = null);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<ImageLimitsDto> GetImageLimitsAsync(string userId);
    Task<bool> CanUserUploadImageAsync(string userId, ImageContext context, int? contextId = null);

    // Car Images
    Task<IEnumerable<CarImageDto>> GetCarImagesAsync(int userCarId);
    Task<CarImageDto?> GetCarImageByIdAsync(int carImageId);
    Task<CarImageDto> AddCarImageAsync(string userId, int carId, IFormFile file, string? caption = null);
    Task<CarImageDto?> UpdateCarImageAsync(string userId, int carImageId, UpdateCarImageRequest request);
    Task<bool> DeleteCarImageAsync(string userId, int carImageId);
    Task<bool> SetCarPrimaryImageAsync(string userId, int carId, int carImageId);

    // Journey Post Images (integrated with existing post creation)
    Task<bool> ValidatePostImagesAsync(string userId, List<CreateJourneyPostImageRequest> images);

    // Image URL Generation
    Task<string> GenerateSecureUploadUrlAsync(string fileName, ImageContext context);
    Task<string> GetOptimizedImageUrlAsync(string originalUrl, int? width = null, int? height = null);
}
