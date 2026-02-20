using Paddokk.Data;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using SkiaSharp;
using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Services;

public class ImageService : IImageService
{
    private readonly PaddokkDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ImageService> _logger;

    // Image size configurations
    private readonly Dictionary<string, int> _imageSizes = new()
        {
            { "thumbnail", 150 },
            { "medium", 1024 },
            { "full", 1920 }
        };

    // File size limits (in bytes)
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    private readonly string[] _allowedFormats = { "image/jpeg", "image/png", "image/webp" };

    public ImageService(
        PaddokkDbContext context,
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        ILogger<ImageService> logger)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ImageUploadDto> UploadImageAsync(IFormFile file, ImageContext context, int? contextId = null, string? caption = null)
    {
        // Validate file
        await ValidateImageFileAsync(file);

        // Generate unique filename
        var fileName = GenerateUniqueFileName(file.FileName);
        var containerName = GetContainerName(context);

        // Get blob container
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        try
        {
            // Process and upload different sizes
            var urls = new Dictionary<string, string>();

            using var originalStream = file.OpenReadStream();
            using var codec = SKCodec.Create(originalStream);

            if (codec == null)
                throw new InvalidOperationException("Failed to decode image");

            using var originalBitmap = SKBitmap.Decode(codec);

            if (originalBitmap == null)
                throw new InvalidOperationException("Failed to create bitmap from image");

            // Upload original/full size
            var fullUrl = await UploadImageSizeAsync(containerClient, originalBitmap, fileName, "full");
            urls["full"] = fullUrl;

            // Upload medium size
            var mediumUrl = await UploadImageSizeAsync(containerClient, originalBitmap, fileName, "medium");
            urls["medium"] = mediumUrl;

            // Upload thumbnail
            var thumbnailUrl = await UploadImageSizeAsync(containerClient, originalBitmap, fileName, "thumbnail");
            urls["thumbnail"] = thumbnailUrl;

            _logger.LogInformation("Successfully uploaded image {FileName} with sizes: {Sizes}",
                fileName, string.Join(", ", urls.Keys));

            return new ImageUploadDto
            {
                ImageUrl = fullUrl,
                ThumbnailUrl = thumbnailUrl,
                MediumUrl = mediumUrl,
                FullUrl = fullUrl,
                FileName = fileName,
                FileSizeBytes = file.Length,
                Width = originalBitmap.Width,
                Height = originalBitmap.Height,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image {FileName}", fileName);
            throw new InvalidOperationException("Failed to upload image", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var pathParts = uri.AbsolutePath.Split('/');
            var containerName = pathParts[1];
            var blobName = string.Join('/', pathParts.Skip(2));

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Extract base name and extension
            var suffixedName = Path.GetFileNameWithoutExtension(blobName);
            var splitBaseName = suffixedName.Split('_');
            var baseName = string.Join('_', splitBaseName.Take(splitBaseName.Length - 1));
            var extension = Path.GetExtension(blobName);

            

            foreach (var size in _imageSizes.Keys)
            {
                var sizedBlobName = $"{baseName}_{size}{extension}";
                await containerClient.DeleteBlobIfExistsAsync(sizedBlobName);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image {ImageUrl}", imageUrl);
            return false;
        }
    }

    public async Task<ImageLimitsDto> GetImageLimitsAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var (maxPerPost, maxPerCar) = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => (1, 3),
            SubscriptionTier.Silver => (3, 5),
            SubscriptionTier.Gold => (5, 7),
            SubscriptionTier.Platinum => (7, 10),
            SubscriptionTier.Diamond => (10, 10),

            _ => (1, 3)
        };

        return new ImageLimitsDto
        {
            MaxImagesPerPost = maxPerPost,
            MaxImagesPerCar = maxPerCar,
            MaxFileSizeBytes = MaxFileSizeBytes,
            AllowedFormats = _allowedFormats,
            SubscriptionTier = user.SubscriptionTier
        };
    }

    public async Task<bool> CanUserUploadImageAsync(string userId, ImageContext context, int? contextId = null)
    {
        var limits = await GetImageLimitsAsync(userId);

        switch (context)
        {
            case ImageContext.Car:
                if (contextId.HasValue)
                {
                    var carImageCount = await _context.UserCarImages
                        .CountAsync(i => i.UserCarId == contextId.Value);
                    return carImageCount < limits.MaxImagesPerCar;
                }
                return true;

            case ImageContext.JourneyPost:
                // This will be validated during post creation
                return true;

            case ImageContext.UserAvatar:
                return true;

            default:
                return false;
        }
    }

    // Car Image Methods
    public async Task<IEnumerable<CarImageDto>> GetCarImagesAsync(int userCarId)
    {
        var images = await _context.UserCarImages
            .Where(i => i.UserCarId == userCarId)
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync();

        return images.Select(MapToCarImageDto);
    }

    public async Task<CarImageDto?> GetCarImageByIdAsync(int carImageId)
    {
        var image = await _context.UserCarImages
            .FirstOrDefaultAsync(i => i.Id == carImageId);

        return image != null ? MapToCarImageDto(image) : null;
    }

    public async Task<CarImageDto> AddCarImageAsync(string userId, int carId, IFormFile file, string? caption = null)
    {
        // Validate user owns the car
        var userCar = await _context.UserCars
            .FirstOrDefaultAsync(c => c.Id == carId && c.UserId == userId && c.IsActive);

        if (userCar == null)
            throw new ArgumentException("Car not found or not owned by user");

        // Check image limits
        if (!await CanUserUploadImageAsync(userId, ImageContext.Car, carId))
            throw new InvalidOperationException("Image limit reached for this car");

        // Upload image
        var uploadResult = await UploadImageAsync(file, ImageContext.Car, carId, caption);

        // Determine if this should be primary (first image)
        var existingImageCount = await _context.UserCarImages.CountAsync(i => i.UserCarId == carId);
        var isPrimary = existingImageCount == 0;

        // Create car image record
        var carImage = new UserCarImage
        {
            UserCarId = carId,
            ImageUrl = uploadResult.FullUrl,
            ThumbnailUrl = uploadResult.ThumbnailUrl,
            MediumUrl = uploadResult.MediumUrl,
            Caption = caption,
            SortOrder = existingImageCount,
            IsPrimary = isPrimary,
            FileSizeBytes = uploadResult.FileSizeBytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            ContentType = uploadResult.ContentType,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserCarImages.Add(carImage);

        // Update car's primary image URL if this is primary
        if (isPrimary)
        {
            userCar.PrimaryImageUrl = uploadResult.MediumUrl;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} added image to car {CarId}", userId, carId);

        return MapToCarImageDto(carImage);
    }

    public async Task<CarImageDto?> UpdateCarImageAsync(string userId, int carImageId, UpdateCarImageRequest request)
    {
        var carImage = await _context.UserCarImages
            .Include(i => i.UserCar)
            .FirstOrDefaultAsync(i => i.Id == carImageId && i.UserCar.UserId == userId);

        if (carImage == null)
            return null;

        // Update caption
        if (request.Caption != null)
            carImage.Caption = request.Caption;

        // Update sort order
        if (request.SortOrder.HasValue)
            carImage.SortOrder = request.SortOrder.Value;

        // Handle primary image change
        if (request.IsPrimary == true && !carImage.IsPrimary)
        {
            // Unset other primary images for this car
            await _context.UserCarImages
                .Where(i => i.UserCarId == carImage.UserCarId && i.IsPrimary)
                .ExecuteUpdateAsync(i => i.SetProperty(p => p.IsPrimary, false));

            carImage.IsPrimary = true;

            // Update car's primary image URL
            carImage.UserCar.PrimaryImageUrl = carImage.MediumUrl;
        }

        await _context.SaveChangesAsync();

        return MapToCarImageDto(carImage);
    }

    public async Task<bool> DeleteCarImageAsync(string userId, int carImageId)
    {
        var carImage = await _context.UserCarImages
            .Include(i => i.UserCar)
            .FirstOrDefaultAsync(i => i.Id == carImageId && i.UserCar.UserId == userId);

        if (carImage == null)
            return false;

        // Delete from blob storage
        await DeleteImageAsync(carImage.ImageUrl);

        // If this was the primary image, set another as primary
        if (carImage.IsPrimary)
        {
            var nextPrimary = await _context.UserCarImages
                .Where(i => i.UserCarId == carImage.UserCarId && i.Id != carImageId)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefaultAsync();

            if (nextPrimary != null)
            {
                nextPrimary.IsPrimary = true;
                carImage.UserCar.PrimaryImageUrl = nextPrimary.MediumUrl;
            }
            else
            {
                carImage.UserCar.PrimaryImageUrl = null;
            }
        }

        _context.UserCarImages.Remove(carImage);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetCarPrimaryImageAsync(string userId, int carId, int carImageId)
    {
        var carImage = await _context.UserCarImages
            .Include(i => i.UserCar)
            .FirstOrDefaultAsync(i => i.Id == carImageId &&
                                    i.UserCarId == carId &&
                                    i.UserCar.UserId == userId);

        if (carImage == null)
            return false;

        // Unset current primary
        await _context.UserCarImages
            .Where(i => i.UserCarId == carId && i.IsPrimary)
            .ExecuteUpdateAsync(i => i.SetProperty(p => p.IsPrimary, false));

        // Set new primary
        carImage.IsPrimary = true;
        carImage.UserCar.PrimaryImageUrl = carImage.MediumUrl;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidatePostImagesAsync(string userId, List<CreateJourneyPostImageRequest> images)
    {
        if (!images.Any())
            return true;

        var limits = await GetImageLimitsAsync(userId);

        if (images.Count > limits.MaxImagesPerPost)
        {
            throw new InvalidOperationException($"Too many images. Maximum allowed: {limits.MaxImagesPerPost}");
        }

        // Validate all image URLs exist and are accessible
        foreach (var image in images)
        {
            if (string.IsNullOrEmpty(image.ImageUrl))
                throw new ArgumentException("Image URL cannot be empty");

            // Additional validation can be added here
        }

        return true;
    }

    // Helper Methods
    private async Task ValidateImageFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException($"File too large. Maximum size: {MaxFileSizeBytes / (1024 * 1024)}MB");

        if (!_allowedFormats.Contains(file.ContentType.ToLower()))
            throw new ArgumentException($"Invalid file format. Allowed: {string.Join(", ", _allowedFormats)}");

        // Validate file is actually an image using SkiaSharp
        try
        {
            using var stream = file.OpenReadStream();
            using var codec = SKCodec.Create(stream);

            if (codec == null)
                throw new ArgumentException("Invalid image file");

            var info = codec.Info;

            if (info.Width < 100 || info.Height < 100)
                throw new ArgumentException("Image too small. Minimum size: 100x100 pixels");

            if (info.Width > 4000 || info.Height > 4000)
                throw new ArgumentException("Image too large. Maximum size: 4000x4000 pixels");
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            throw new ArgumentException("Invalid image file");
        }
    }

    private async Task<string> UploadImageSizeAsync(BlobContainerClient containerClient, SKBitmap originalBitmap, string baseFileName, string sizeKey)
    {
        var targetSize = _imageSizes[sizeKey];
        var fileName = $"{Path.GetFileNameWithoutExtension(baseFileName)}_{sizeKey}.webp";

        // Calculate new dimensions maintaining aspect ratio
        var (newWidth, newHeight) = CalculateResizeDimensions(originalBitmap.Width, originalBitmap.Height, targetSize);

        // Create resized bitmap
        using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

        if (resizedBitmap == null)
            throw new InvalidOperationException("Failed to resize image");

        // Convert to WebP
        using var image = SKImage.FromBitmap(resizedBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Webp, 85); // 85% quality

        if (data == null)
            throw new InvalidOperationException("Failed to encode image as WebP");

        // Upload to blob storage
        using var outputStream = data.AsStream();
        var blobClient = containerClient.GetBlobClient(fileName);
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/webp" };

        await blobClient.UploadAsync(outputStream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });

        return blobClient.Uri.ToString();
    }

    private (int width, int height) CalculateResizeDimensions(int originalWidth, int originalHeight, int maxSize)
    {
        if (originalWidth <= maxSize && originalHeight <= maxSize)
            return (originalWidth, originalHeight);

        var aspectRatio = (double)originalWidth / originalHeight;

        if (originalWidth > originalHeight)
        {
            // Landscape
            return (maxSize, (int)(maxSize / aspectRatio));
        }
        else
        {
            // Portrait or square
            return ((int)(maxSize * aspectRatio), maxSize);
        }
    }

    private string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var guid = Guid.NewGuid().ToString("N")[..8];
        return $"{timestamp}_{guid}{extension}";
    }

    private string GetContainerName(ImageContext context)
    {
        return context switch
        {
            ImageContext.Car => "car-images",
            ImageContext.JourneyPost => "post-images",
            ImageContext.UserAvatar => "avatars",
            _ => "misc-images"
        };
    }

    private CarImageDto MapToCarImageDto(UserCarImage carImage)
    {
        return new CarImageDto
        {
            Id = carImage.Id,
            UserCarId = carImage.UserCarId,
            ImageUrl = carImage.ImageUrl,
            ThumbnailUrl = carImage.ThumbnailUrl,
            MediumUrl = carImage.MediumUrl,
            Caption = carImage.Caption,
            SortOrder = carImage.SortOrder,
            IsPrimary = carImage.IsPrimary,
            CreatedAt = carImage.CreatedAt
        };
    }

    // Placeholder methods for future implementation
    public async Task<string> GenerateSecureUploadUrlAsync(string fileName, ImageContext context)
    {
        // Future: Generate SAS URLs for direct client upload
        await Task.CompletedTask;
        return string.Empty;
    }

    public async Task<string> GetOptimizedImageUrlAsync(string originalUrl, int? width = null, int? height = null)
    {
        // Future: Image transformation service integration
        await Task.CompletedTask;
        return originalUrl;
    }
}
