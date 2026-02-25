using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/cars/{carId}/images")]
[Authorize]
public class CarImagesController(IImageService imageService) : ControllerBase
{
    private readonly IImageService _imageService = imageService;

    [HttpGet]
    [EndpointSummary("Get all images for a car")]
    public async Task<IEnumerable<CarImageDto>> GetCarImages(
        int carId, CancellationToken cancellationToken) =>
        await _imageService.GetCarImagesAsync(carId, cancellationToken);

    [HttpGet("{imageId}")]
    [EndpointSummary("Get a specific car image by ID")]
    public async Task<CarImageDto> GetCarImage(
        int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.GetCarImageByIdAsync(imageId, carId, cancellationToken);

    [HttpPost]
    [EndpointSummary("Upload a new image for the car")]
    public async Task<CarImageDto> UploadCarImage(
        int carId, IFormFile file, CancellationToken cancellationToken, [FromForm] string? caption = null) =>
        await _imageService.AddCarImageAsync(User.GetUserId(), carId, file, cancellationToken, caption);

    [HttpPut("{imageId}")]
    [EndpointSummary("Update caption or sort order of a car image")]
    public async Task<CarImageDto> UpdateCarImage(
        int imageId, [FromBody] UpdateCarImageRequest request, CancellationToken cancellationToken) =>
        await _imageService.UpdateCarImageAsync(User.GetUserId(), imageId, request, cancellationToken);

    [HttpDelete("{imageId}")]
    [EndpointSummary("Delete a car image")]
    public async Task DeleteCarImage(
        int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.DeleteCarImageAsync(User.GetUserId(), carId, imageId, cancellationToken);

    [HttpPut("{imageId}/setprimary")]
    [EndpointSummary("Set an image as the primary image for the car")]
    public async Task SetPrimaryImage(int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.SetCarPrimaryImageAsync(User.GetUserId(), carId, imageId, cancellationToken);

    [HttpGet("canupload")]
    [EndpointSummary("Check if the current user can upload more images for this car")]
    public async Task<CanUploadImageResponse> CanUploadImage(int carId, CancellationToken cancellationToken) =>
        await _imageService.GetUploadStatusAsync(User.GetUserId(), carId, cancellationToken);
}
