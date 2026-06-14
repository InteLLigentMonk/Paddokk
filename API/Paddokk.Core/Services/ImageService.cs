using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;
using SkiaSharp;

namespace Paddokk.Core.Services;

public class ImageService : IImageService
{
    private readonly IUserRepository _userRepository;
    private readonly ICarRepository _carRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<ImageService> _logger;

    // Master image is sized to serve as the origin for Vercel Image Optimization,
    // which transforms it to any smaller size/format on demand at the edge.
    private const int MasterMaxDimension = 2400;
    private const int WebpQuality = 90;
    private const string ImmutableCacheControl = "public, max-age=31536000, immutable";

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    private readonly string[] _allowedFormats = { "image/jpg", "image/jpeg", "image/png", "image/webp" };

    public ImageService(
        IUserRepository userRepository,
        BlobServiceClient blobServiceClient,
        ILogger<ImageService> logger,
        IImageRepository imageRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _imageRepository = imageRepository;
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImageUploadDto> UploadImageAsync(IFormFile file, ImageContext context, CancellationToken cancellationToken, int? contextId = null, string? caption = null)
    {
        ValidateImageFile(file);

        var fileName = GenerateUniqueFileName(file.FileName);
        var containerName = GetContainerName(context);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        try
        {
            using var originalStream = file.OpenReadStream();
            using var codec = SKCodec.Create(originalStream)
                ?? throw new InvalidOperationException("Failed to decode image");

            using var originalBitmap = SKBitmap.Decode(codec)
                ?? throw new InvalidOperationException("Failed to create bitmap from image");

            var imageUrl = await UploadMasterImageAsync(containerClient, originalBitmap, fileName, cancellationToken);

            _logger.LogInformation("Successfully uploaded image {FileName}", fileName);

            return new ImageUploadDto
            {
                ImageUrl = imageUrl,
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

    public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken)
    {
        try
        {
            var parsed = BlobUrlParser.Parse(imageUrl)
                ?? throw new InvalidOperationException($"Invalid image URL: {imageUrl}");

            var containerClient = _blobServiceClient.GetBlobContainerClient(parsed.Container);
            await containerClient.DeleteBlobIfExistsAsync(parsed.BlobName, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image {ImageUrl}", imageUrl);
            throw new InvalidOperationException("Failed to delete image", ex);
        }
    }

    public async Task<ImageLimitsDto> GetImageLimitsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
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

    public async Task<bool> CanUserUploadImageAsync(string userId, ImageContext context, CancellationToken cancellationToken, int? contextId = null)
    {
        var limits = await GetImageLimitsAsync(userId, cancellationToken: cancellationToken);

        switch (context)
        {
            case ImageContext.Car:
                if (contextId.HasValue)
                {
                    var carImageCount = await _imageRepository.GetImageCountByContextAsync(context.ToString(), contextId.Value, cancellationToken);
                    return carImageCount < limits.MaxImagesPerCar;
                }
                return true;

            case ImageContext.JourneyPost:
                return true;

            case ImageContext.UserAvatar:
                return true;

            default:
                return false;
        }
    }

    public async Task<CanUploadImageResponse> GetUploadStatusAsync(string userId, int carId, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);
        if (car is null)
            throw new InvalidOperationException("Car not found or access denied");

        var canUpload = await CanUserUploadImageAsync(userId, ImageContext.Car, cancellationToken, carId);
        var limits = await GetImageLimitsAsync(userId, cancellationToken);
        var imageCount = await _imageRepository.GetImageCountByContextAsync(
            ImageContext.Car.ToString(), carId, cancellationToken);

        return new CanUploadImageResponse
        {
            CanUpload = canUpload,
            CurrentCount = imageCount,
            MaxAllowed = limits.MaxImagesPerCar,
            SubscriptionTier = limits.SubscriptionTier
        };
    }

    // Car Image Methods
    public async Task<CarImagesResponse> GetCarImagesAsync(int carId, CancellationToken cancellationToken)
    {
        var images = await _imageRepository.GetCarImagesAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Failed to retrieve car images");

        return new CarImagesResponse { Images = [.. images.Select(MapToCarImageDto)] };
    }

    public async Task<CarImageDto> GetCarImageByIdAsync(int carImageId, int carId, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetCarImageByIdAsync(carImageId, cancellationToken);
        return image != null ? MapToCarImageDto(image) : throw new InvalidOperationException("Car image not found");
    }

    public async Task<CarImageDto> AddCarImageAsync(string userId, int carId, IFormFile file, CancellationToken cancellationToken, string? caption = null)
    {
        var car = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);
        if (car is null)
            throw new InvalidOperationException("Car not found or access denied");

        if (!await CanUserUploadImageAsync(userId, ImageContext.Car, cancellationToken, carId))
            throw new InvalidOperationException("Image limit reached for this car");

        var uploadResult = await UploadImageAsync(file, ImageContext.Car, cancellationToken, carId, caption)
            ?? throw new InvalidOperationException("Image upload failed");

        var existingImageCount = await _imageRepository.GetImageCountByContextAsync(
            ImageContext.Car.ToString(), carId, cancellationToken);
        var isPrimary = existingImageCount == 0;

        var carImage = new UserCarImage
        {
            UserCarId = carId,
            ImageUrl = uploadResult.ImageUrl,
            Caption = caption,
            SortOrder = existingImageCount,
            IsPrimary = isPrimary,
            FileSizeBytes = uploadResult.FileSizeBytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            ContentType = uploadResult.ContentType,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _imageRepository.AddCarImageAsync(carImage, cancellationToken);

            if (isPrimary)
            {
                await _imageRepository.SetPrimaryImageAsync(carId, carImage.Id, cancellationToken);
                await _carRepository.UpdatePrimaryImageUrlAsync(carId, uploadResult.ImageUrl, cancellationToken);
            }
        }, cancellationToken);

        _logger.LogInformation("User {UserId} added image to car {CarId}", userId, carId);

        return MapToCarImageDto(carImage);
    }

    public async Task<CarImageDto> UpdateCarImageAsync(string userId, int carImageId, UpdateCarImageRequest request, CancellationToken cancellationToken)
    {
        var carImage = await _imageRepository.GetCarImageByIdAsync(carImageId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Car image not found");

        if (request.Caption != null)
            carImage.Caption = request.Caption;

        if (request.SortOrder.HasValue)
            carImage.SortOrder = request.SortOrder.Value;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (request.IsPrimary == true && !carImage.IsPrimary)
            {
                await _imageRepository.SetPrimaryImageAsync(carImage.UserCarId, carImageId, cancellationToken);
                await _carRepository.UpdatePrimaryImageUrlAsync(carImage.UserCarId, carImage.ImageUrl, cancellationToken);
            }

            await _imageRepository.UpdateCarImageAsync(carImage, cancellationToken);
        }, cancellationToken);

        return MapToCarImageDto(carImage);
    }

    public async Task DeleteCarImageAsync(string userId, int carId, int carImageId, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);
        if (car is null)
            throw new InvalidOperationException("Car not found or access denied");

        var carImage = await _imageRepository.GetCarImageByIdAsync(carImageId, userId, cancellationToken)
             ?? throw new InvalidOperationException("Car image not found");

        await DeleteImageAsync(carImage.ImageUrl, cancellationToken);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (carImage.IsPrimary)
            {
                var nextPrimary = await _imageRepository.GetNextPrimaryImageAsync(
                    carImage.UserCarId, carImageId, cancellationToken);

                if (nextPrimary is not null)
                    await _imageRepository.SetPrimaryImageAsync(carImage.UserCarId, nextPrimary.Id, cancellationToken);

                await _carRepository.UpdatePrimaryImageUrlAsync(carImage.UserCarId, nextPrimary?.ImageUrl, cancellationToken);
            }

            await _imageRepository.DeleteCarImageAsync(carImage.Id, cancellationToken);
        }, cancellationToken);
    }

    public async Task SetCarPrimaryImageAsync(string userId, int carId, int carImageId, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetUserCarByIdAsync(userId, carId, cancellationToken);
        if (car is null)
            throw new InvalidOperationException("Car not found or access denied");

        var image = await _imageRepository.GetCarImageByIdAsync(carImageId, cancellationToken);
        if (image is null || image.UserCarId != carId)
            throw new InvalidOperationException("Car image not found or does not belong to the specified car");

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _imageRepository.SetPrimaryImageAsync(carId, carImageId, cancellationToken);
            await _carRepository.UpdatePrimaryImageUrlAsync(carId, image.ImageUrl, cancellationToken);
        }, cancellationToken);
    }

    public async Task ValidatePostImagesAsync(string userId, List<CreateJourneyPostImageRequest> images, CancellationToken cancellationToken)
    {
        var limits = await GetImageLimitsAsync(userId, cancellationToken);

        if (images.Count > limits.MaxImagesPerPost)
        {
            throw new InvalidOperationException($"Too many images. Maximum allowed: {limits.MaxImagesPerPost}");
        }

        foreach (var image in images)
        {
            if (string.IsNullOrEmpty(image.ImageUrl))
                throw new ArgumentException("Image URL cannot be empty");
        }
    }

    // Helper Methods
    private void ValidateImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException($"File too large. Maximum size: {MaxFileSizeBytes / (1024 * 1024)}MB");

        if (!_allowedFormats.Contains(file.ContentType.ToLower()))
            throw new ArgumentException($"Invalid file format. Allowed: {string.Join(", ", _allowedFormats)}");

        try
        {
            using var stream = file.OpenReadStream();
            using var codec = SKCodec.Create(stream) ?? throw new ArgumentException("Invalid image file");
            var info = codec.Info;

            if (info.Width < 100 || info.Height < 100)
                throw new ArgumentException("Image too small. Minimum size: 100x100 pixels");

            if (info.Width > 4000 || info.Height > 4000)
                throw new ArgumentException("Image too large. Maximum size: 4000x4000 pixels");
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException("Invalid image file");
        }
    }

    private static async Task<string> UploadMasterImageAsync(BlobContainerClient containerClient, SKBitmap originalBitmap, string baseFileName, CancellationToken cancellationToken)
    {
        var fileName = $"{Path.GetFileNameWithoutExtension(baseFileName)}.webp";

        var (newWidth, newHeight) = CalculateResizeDimensions(originalBitmap.Width, originalBitmap.Height, MasterMaxDimension);

        using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), new SKSamplingOptions(SKCubicResampler.Mitchell))
            ?? throw new InvalidOperationException("Failed to resize image");

        using var image = SKImage.FromBitmap(resizedBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Webp, WebpQuality)
            ?? throw new InvalidOperationException("Failed to encode image as WebP");

        using var outputStream = data.AsStream();
        var blobClient = containerClient.GetBlobClient(fileName);
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "image/webp",
            CacheControl = ImmutableCacheControl
        };

        await blobClient.UploadAsync(outputStream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        }, cancellationToken);

        return blobClient.Uri.ToString();
    }

    private static (int width, int height) CalculateResizeDimensions(int originalWidth, int originalHeight, int maxSize)
    {
        if (originalWidth <= maxSize && originalHeight <= maxSize)
            return (originalWidth, originalHeight);

        var aspectRatio = (double)originalWidth / originalHeight;

        if (originalWidth > originalHeight)
        {
            return (maxSize, (int)(maxSize / aspectRatio));
        }
        else
        {
            return ((int)(maxSize * aspectRatio), maxSize);
        }
    }

    private static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var guid = Guid.NewGuid().ToString("N")[..8];
        return $"{timestamp}_{guid}{extension}";
    }

    private static string GetContainerName(ImageContext context)
    {
        return context switch
        {
            ImageContext.Car => "car-images",
            ImageContext.JourneyPost => "post-images",
            ImageContext.UserAvatar => "avatars",
            _ => "misc-images"
        };
    }

    private static CarImageDto MapToCarImageDto(UserCarImage carImage)
    {
        return new CarImageDto
        {
            Id = carImage.Id,
            UserCarId = carImage.UserCarId,
            ImageUrl = carImage.ImageUrl,
            Caption = carImage.Caption,
            SortOrder = carImage.SortOrder,
            IsPrimary = carImage.IsPrimary,
            CreatedAt = carImage.CreatedAt
        };
    }
}
