using Paddokk.Api.Extensions;
using Paddokk.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/cars/{carId}/images")]
[Authorize]
public class CarImagesController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly ICarService _carService;
    private readonly ILogger<CarImagesController> _logger;

    public CarImagesController(
        IImageService imageService,
        ICarService carService,
        ILogger<CarImagesController> logger)
    {
        _imageService = imageService;
        _carService = carService;
        _logger = logger;
    }

    /// <summary>
    /// Get all images for a car
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarImageDto>>> GetCarImages(int carId)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var images = await _imageService.GetCarImagesAsync(carId);
        return Ok(images);
    }

    /// <summary>
    /// Upload new image for car
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CarImageDto>> UploadCarImage(int carId, IFormFile file, [FromForm] string? caption = null)
    {
        try
        {
            var userId = User.GetUserId();

            // Verify user owns the car
            var car = await _carService.GetUserCarByIdAsync(userId, carId);
            if (car == null)
                return NotFound(new { message = "Car not found" });

            var result = await _imageService.AddCarImageAsync(userId, carId, file, caption);
            return CreatedAtAction(nameof(GetCarImage), new { carId, imageId = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get specific car image
    /// </summary>
    [HttpGet("{imageId}")]
    public async Task<ActionResult<CarImageDto>> GetCarImage(int carId, int imageId)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var image = await _imageService.GetCarImageByIdAsync(imageId);
        if (image == null || image.UserCarId != carId)
            return NotFound(new { message = "Image not found" });

        return Ok(image);
    }

    /// <summary>
    /// Update car image details
    /// </summary>
    [HttpPut("{imageId}")]
    public async Task<ActionResult<CarImageDto>> UpdateCarImage(int carId, int imageId, [FromBody] UpdateCarImageRequest request)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var result = await _imageService.UpdateCarImageAsync(userId, imageId, request);
        if (result == null)
            return NotFound(new { message = "Image not found" });

        return Ok(result);
    }

    /// <summary>
    /// Delete car image
    /// </summary>
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteCarImage(int carId, int imageId)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var result = await _imageService.DeleteCarImageAsync(userId, imageId);
        if (!result)
            return NotFound(new { message = "Image not found" });

        return NoContent();
    }

    /// <summary>
    /// Set image as primary for car
    /// </summary>
    [HttpPut("{imageId}/setprimary")]
    public async Task<IActionResult> SetPrimaryImage(int carId, int imageId)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var result = await _imageService.SetCarPrimaryImageAsync(userId, carId, imageId);
        if (!result)
            return NotFound(new { message = "Image not found" });

        return Ok(new { message = "Primary image updated successfully" });
    }

    /// <summary>
    /// Check if user can upload more images for this car
    /// </summary>
    [HttpGet("canupload")]
    public async Task<ActionResult<object>> CanUploadImage(int carId)
    {
        var userId = User.GetUserId();

        // Verify user owns the car
        var car = await _carService.GetUserCarByIdAsync(userId, carId);
        if (car == null)
            return NotFound(new { message = "Car not found" });

        var canUpload = await _imageService.CanUserUploadImageAsync(userId, ImageContext.Car, carId);
        var limits = await _imageService.GetImageLimitsAsync(userId);
        var currentImages = await _imageService.GetCarImagesAsync(carId);

        return Ok(new
        {
            canUpload,
            currentCount = currentImages.Count(),
            maxAllowed = limits.MaxImagesPerCar,
            subscriptionTier = limits.SubscriptionTier.ToString()
        });
    }
}
