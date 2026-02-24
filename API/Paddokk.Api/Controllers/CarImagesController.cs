using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/cars/{carId}/images")]
[Authorize]
public class CarImagesController(IImageService imageService, ICarService carService) : ControllerBase
{
    private readonly IImageService _imageService = imageService;
    private readonly ICarService _carService = carService;

    /// <summary>
    /// Get all images for a car
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CarImageDto>> GetCarImages(
        int carId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        return await _imageService.GetCarImagesAsync(carId, userId, cancellationToken);
    }

    /// <summary>
    /// Upload new image for car
    /// </summary>
    [HttpPost]
    public async Task<CarImageDto> UploadCarImage(
        int carId, IFormFile file, CancellationToken cancellationToken, [FromForm] string? caption = null)
    {
        var userId = User.GetUserId();
        return await _imageService.AddCarImageAsync(userId, carId, file, cancellationToken, caption);
    }

    /// <summary>
    /// Get specific car image
    /// </summary>
    [HttpGet("{imageId}")]
    public async Task<CarImageDto> GetCarImage(
        int carId, int imageId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        return await _imageService.GetCarImageByIdAsync(imageId, carId, userId, cancellationToken);
    }

    /// <summary>
    /// Update car image details
    /// </summary>
    [HttpPut("{imageId}")]
    public async Task<CarImageDto> UpdateCarImage(
        int imageId, [FromBody] UpdateCarImageRequest request, CancellationToken cancellationToken) => 
            await _imageService.UpdateCarImageAsync(User.GetUserId(), imageId, request, cancellationToken);

    /// <summary>
    /// Delete car image
    /// </summary>
    [HttpDelete("{imageId}")]
    public async Task DeleteCarImage(
        int carId, int imageId, CancellationToken cancellationToken) =>
            await _imageService.DeleteCarImageAsync(User.GetUserId(), carId, imageId, cancellationToken);

    /// <summary>
    /// Set image as primary for car
    /// </summary>
    [HttpPut("{imageId}/setprimary")]
    public async Task SetPrimaryImage(int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.SetCarPrimaryImageAsync(User.GetUserId(), carId, imageId, cancellationToken);












    /// <summary>
    /// Check if user can upload more images for this car
    /// </summary>
    [HttpGet("canupload")]
    public async Task<ActionResult<object>> CanUploadImage(int carId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _carService.UserOwnsCarAsync(userId, carId, cancellationToken);

        var canUpload = await _imageService.CanUserUploadImageAsync(userId, ImageContext.Car, cancellationToken, carId);
        var limits = await _imageService.GetImageLimitsAsync(userId, cancellationToken);
        var currentImages = await _imageService.GetCarImagesAsync(carId, userId, cancellationToken);

        return Ok(new
        {
            canUpload,
            currentCount = currentImages.Count(),
            maxAllowed = limits.MaxImagesPerCar,
            subscriptionTier = limits.SubscriptionTier.ToString()
        });
    }
}
