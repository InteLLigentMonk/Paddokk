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
    public async Task<CarImagesResponse> GetCarImages(
        int carId, CancellationToken cancellationToken) =>
        new() { Images = [.. await _imageService.GetCarImagesAsync(carId, cancellationToken)] };

    [HttpGet("{imageId}")]
    [EndpointSummary("Get a specific car image by ID")]
    public async Task<CarImageDto> GetCarImage(
        int carId, int imageId, CancellationToken cancellationToken) =>
        await _imageService.GetCarImageByIdAsync(imageId, carId, cancellationToken);

    [HttpPost]
    [EndpointSummary("Upload a new image for the car")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<CarImageDto> UploadCarImage(
        int carId, [FromForm] UploadCarImageRequest request, CancellationToken cancellationToken) =>
        await _imageService.AddCarImageAsync(User.GetUserId(), carId, request.File, cancellationToken, request.Caption);

    [HttpPut("{imageId}")]
    [EndpointSummary("Update caption or sort order of a car image")]
    public async Task<CarImageDto> UpdateCarImage(
        int carId, int imageId, [FromBody] UpdateCarImageRequest request, CancellationToken cancellationToken) =>
        await _imageService.UpdateCarImageAsync(User.GetUserId(), imageId, request, cancellationToken);

    [HttpDelete("{imageId}")]
    [EndpointSummary("Delete a car image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task DeleteCarImage(
        int carId, int imageId, CancellationToken cancellationToken)
    {
        await _imageService.DeleteCarImageAsync(User.GetUserId(), carId, imageId, cancellationToken);
        Response.StatusCode = StatusCodes.Status204NoContent;
    }

    [HttpPut("{imageId}/setprimary")]
    [EndpointSummary("Set an image as the primary image for the car")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task SetPrimaryImage(int carId, int imageId, CancellationToken cancellationToken)
    {
        await _imageService.SetCarPrimaryImageAsync(User.GetUserId(), carId, imageId, cancellationToken);
        Response.StatusCode = StatusCodes.Status204NoContent;
    }

    [HttpGet("canupload")]
    [EndpointSummary("Check if the current user can upload more images for this car")]
    public async Task<CanUploadImageResponse> CanUploadImage(int carId, CancellationToken cancellationToken) =>
        await _imageService.GetUploadStatusAsync(User.GetUserId(), carId, cancellationToken);
}
