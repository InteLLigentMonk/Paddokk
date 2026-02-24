using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(IImageService imageService, ILogger<ImagesController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Upload image with automatic optimization and multiple sizes
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ImageUploadDto>> UploadImage([FromForm] ImageUploadRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();

            // Check if user can upload images for this context
            if (!await _imageService.CanUserUploadImageAsync(userId, request.Context, cancellationToken, request.ContextId))
            {
                var limits = await _imageService.GetImageLimitsAsync(userId, cancellationToken);
                return BadRequest(new
                {
                    message = $"Image limit reached. Your plan allows {GetLimitForContext(limits, request.Context)} images.",
                    limits
                });
            }

            var result = await _imageService.UploadImageAsync(
                request.File,
                request.Context,
                cancellationToken,
                request.ContextId,
                request.Caption);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image for user {UserId}", User.GetUserId());
            return StatusCode(500, new { message = "Failed to upload image" });
        }
    }

    /// <summary>
    /// Get user's image upload limits based on subscription
    /// </summary>
    [HttpGet("limits")]
    public async Task<ActionResult<ImageLimitsDto>> GetImageLimits(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var limits = await _imageService.GetImageLimitsAsync(userId, cancellationToken);
        return Ok(limits);
    }

    /// <summary>
    /// Delete uploaded image
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteImage([FromQuery] string imageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return BadRequest(new { message = "Image URL is required" });

        var result = await _imageService.DeleteImageAsync(imageUrl, cancellationToken);

        if (!result)
            return NotFound(new { message = "Image not found or could not be deleted" });

        return Ok(new { message = "Image deleted successfully" });
    }

    private static string GetLimitForContext(ImageLimitsDto limits, ImageContext context)
    {
        return context switch
        {
            ImageContext.Car => limits.MaxImagesPerCar.ToString(),
            ImageContext.JourneyPost => limits.MaxImagesPerPost.ToString(),
            ImageContext.UserAvatar => "1",
            _ => "unknown"
        };
    }
}
