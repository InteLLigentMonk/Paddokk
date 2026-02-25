using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/limits")]
[Authorize]
public class LimitsController(IImageService imageService, ICarService carService) : ControllerBase
{
    private readonly IImageService _imageService = imageService;
    private readonly ICarService _carService = carService;

    [HttpGet("images")]
    [EndpointSummary("Get the current user's image upload limits across all contexts")]
    public async Task<ImageLimitsDto> GetImageLimits(CancellationToken cancellationToken) =>
        await _imageService.GetImageLimitsAsync(User.GetUserId(), cancellationToken);

    [HttpGet("cars")]
    [EndpointSummary("Get the current user's car limit status")]
    public async Task<CarLimitDto> GetCarLimits(CancellationToken cancellationToken) =>
        await _carService.CanUserAddCarAsync(User.GetSubscriptionTier(), User.GetUserId(), cancellationToken);
}
