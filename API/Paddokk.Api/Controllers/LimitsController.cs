using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Features.Cars.Queries.GetCarLimits;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/limits")]
[Authorize]
public class LimitsController(ISender sender, IImageService imageService) : ApiControllerBase
{
    [HttpGet("images")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the current user's image upload limits across all contexts")]
    public async Task<ImageLimitsDto> GetImageLimits(CancellationToken ct) =>
        await imageService.GetImageLimitsAsync(User.GetUserId(), ct);

    [HttpGet("cars")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the current user's car limit status")]
    public async Task<CarLimitDto> GetCarLimits(CancellationToken ct) =>
        await sender.Send(new GetCarLimitsQuery(User.GetSubscriptionTier()), ct);
}
