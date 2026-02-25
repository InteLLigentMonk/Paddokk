using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/cars/{carId}/images")]
[Authorize]
public class CarImagesController(IImageService imageService) : ControllerBase
{
    private readonly IImageService _imageService = imageService;

    [HttpGet]
    [SwaggerOperation(Summary = "Get all images for a car")]
    public async Task<IEnumerable<CarImageDto>> GetCarImages(
        int carId, CancellationToken cancellationToken) => 
        await _imageService.GetCarImagesAsync(carId, User.GetUserId(), cancellationToken);

    [HttpPost]
    [SwaggerOperation(Summary = "Upload new image for car")]
    public async Task<CarImageDto> UploadCarImage(
        int carId, IFormFile file, CancellationToken cancellationToken, [FromForm] string? caption = null) => 
        await _imageService.AddCarImageAsync(User.GetUserId(), carId, file, cancellationToken, caption);

    /// <summary>
    /// Get specific car image
    /// </summary>
    [HttpGet("{imageId}")]
    public async Task<CarImageDto> GetCarImage(
        int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.GetCarImageByIdAsync(imageId, carId, User.GetUserId(), cancellationToken);

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
    public async Task<CanUploadImageResponse> CanUploadImage(int carId, CancellationToken cancellationToken) =>
        await _imageService.GetUploadStatusAsync(User.GetUserId(), carId, cancellationToken);
}
