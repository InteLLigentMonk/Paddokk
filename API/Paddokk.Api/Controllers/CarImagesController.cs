using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.CarImages.Commands.DeleteCarImage;
using Paddokk.Core.Features.CarImages.Commands.SetPrimaryImage;
using Paddokk.Core.Features.CarImages.Commands.UpdateCarImage;
using Paddokk.Core.Features.CarImages.Commands.UploadCarImage;
using Paddokk.Core.Features.CarImages.Queries.GetCarImageById;
using Paddokk.Core.Features.CarImages.Queries.GetCarImages;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/cars/{carId}/images")]
[Authorize]
public class CarImagesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get all images for a car")]
    public async Task<ActionResult<CarImagesResponse>> GetCarImages(int carId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCarImagesQuery(carId), ct);
        return OkOrError(result);
    }

    [HttpGet("{imageId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a specific car image by ID")]
    public async Task<ActionResult<CarImageDto>> GetCarImageById(int carId, int imageId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCarImageByIdQuery(carId, imageId), ct);
        return OkOrError(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting("upload")]
    [EndpointSummary("Upload a new image for the car")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CarImageDto>> UploadCarImage(
        int carId, [FromForm] UploadCarImageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UploadCarImageCommand(carId, request.File, request.Caption), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return CreatedAtAction(nameof(GetCarImageById), new { carId, imageId = result.Value!.Id }, result.Value);
    }

    [HttpPut("{imageId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update caption or sort order of a car image")]
    public async Task<ActionResult<CarImageDto>> UpdateCarImage(
        int carId, int imageId, [FromBody] UpdateCarImageRequest body, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateCarImageCommand(carId, imageId, body.Caption, body.SortOrder, body.IsPrimary), ct);
        return OkOrError(result);
    }

    [HttpDelete("{imageId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete a car image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCarImage(int carId, int imageId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCarImageCommand(carId, imageId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpPut("{imageId}/setprimary")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Set an image as the primary image for the car")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPrimaryImage(int carId, int imageId, CancellationToken ct)
    {
        var result = await sender.Send(new SetPrimaryImageCommand(carId, imageId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }
}
