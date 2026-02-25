using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/images")]
[Authorize]
public class ImagesController(IImageService imageService) : ControllerBase
{
    private readonly IImageService _imageService = imageService;

    [HttpGet("limits")]
    [EndpointSummary("Get the current user's image upload limits across all contexts")]
    public async Task<ImageLimitsDto> GetImageLimits(CancellationToken cancellationToken) =>
        await _imageService.GetImageLimitsAsync(User.GetUserId(), cancellationToken);
}
