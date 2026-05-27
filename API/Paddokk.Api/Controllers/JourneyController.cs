using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Journeys.Commands.CreateJourneyPost;
using Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPost;
using Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPostImage;
using Paddokk.Core.Features.Journeys.Commands.UploadJourneyPostImage;
using Paddokk.Core.Features.Journeys.Commands.LikeJourney;
using Paddokk.Core.Features.Journeys.Commands.SubscribeToJourney;
using Paddokk.Core.Features.Journeys.Commands.UnlikeJourney;
using Paddokk.Core.Features.Journeys.Commands.UnsubscribeFromJourney;
using Paddokk.Core.Features.Journeys.Commands.UpdateJourneyPost;
using Paddokk.Core.Features.Journeys.Queries.GetFeaturedJourneys;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyById;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyPostById;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;
using Paddokk.Core.Features.Journeys.Queries.GetJourneysBrowseStats;
using Paddokk.Core.Features.Journeys.Queries.GetTrendingJourneys;
using Paddokk.Core.Features.Journeys.Queries.SearchJourneys;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/journeys")]
public class JourneysController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Search and browse journeys with filtering")]
    public async Task<ActionResult<PagedJourneysResponse>> SearchJourneys(
        [FromQuery] JourneySearchRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SearchJourneysQuery(request), ct);
        return OkOrError(result);
    }

    [HttpGet("browse-stats")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Aggregate stats for the journeys browse page (respects search filters)")]
    public async Task<ActionResult<GetJourneysBrowseStatsResponse>> GetBrowseStats(
        [FromQuery] JourneySearchRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new GetJourneysBrowseStatsQuery(request), ct);
        return OkOrError(result);
    }

    [HttpGet("{journeyId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get specific journey details")]
    public async Task<ActionResult<JourneyDto>> GetJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new GetJourneyByIdQuery(journeyId), ct);
        return OkOrError(result);
    }

    [HttpGet("featured")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get featured journeys (most liked)")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetFeaturedJourneys(CancellationToken ct)
    {
        var result = await sender.Send(new GetFeaturedJourneysQuery(), ct);
        return Ok(result);
    }

    [HttpGet("trending")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get trending journeys (recently active)")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetTrendingJourneys(CancellationToken ct)
    {
        var result = await sender.Send(new GetTrendingJourneysQuery(), ct);
        return Ok(result);
    }

    [HttpPost("{journeyId}/subscribe")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Subscribe to journey for updates")]
    public async Task<IActionResult> SubscribeToJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new SubscribeToJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpDelete("{journeyId}/subscribe")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Unsubscribe from journey")]
    public async Task<IActionResult> UnsubscribeFromJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new UnsubscribeFromJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpPost("{journeyId}/like")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Like a journey")]
    public async Task<IActionResult> LikeJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new LikeJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpDelete("{journeyId}/like")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Unlike a journey")]
    public async Task<IActionResult> UnlikeJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new UnlikeJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpGet("{journeyId}/posts")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get journey posts (timeline)")]
    public async Task<ActionResult<PagedResult<JourneyPostDto>>> GetJourneyPosts(
        int journeyId, CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetJourneyPostsQuery(journeyId, page, pageSize), ct);
        return OkOrError(result);
    }

    [HttpPost("{journeyId}/posts")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Create new post in journey")]
    public async Task<ActionResult<JourneyPostDto>> CreateJourneyPost(
        int journeyId, [FromBody] CreateJourneyPostCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { JourneyId = journeyId }, ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return CreatedAtAction(nameof(GetJourneyPost), new { postId = result.Value!.Id }, result.Value);
    }

    [HttpGet("posts/{postId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get specific journey post")]
    public async Task<ActionResult<JourneyPostDto>> GetJourneyPost(int postId, CancellationToken ct)
    {
        var result = await sender.Send(new GetJourneyPostByIdQuery(postId), ct);
        return OkOrError(result);
    }

    [HttpPut("posts/{postId}")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update journey post (text content only)")]
    public async Task<ActionResult<JourneyPostDto>> UpdateJourneyPost(
        int postId, [FromBody] UpdateJourneyPostCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { PostId = postId }, ct);
        return OkOrError(result);
    }

    [HttpDelete("posts/{postId}")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete journey post")]
    public async Task<IActionResult> DeleteJourneyPost(int postId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteJourneyPostCommand(postId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpPost("{journeyId}/posts/upload-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Upload an image for a journey post (pre-upload before post creation)")]
    public async Task<ActionResult<ImageUploadDto>> UploadJourneyPostImage(
        int journeyId, IFormFile file, CancellationToken ct)
    {
        var result = await sender.Send(new UploadJourneyPostImageCommand(journeyId, file), ct);
        return OkOrError(result);
    }

    [HttpDelete("{journeyId}/posts/upload-image")]
    [Authorize]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete a pre-uploaded journey post image (cleanup orphaned images)")]
    public async Task<IActionResult> DeleteJourneyPostImage(
        int journeyId, [FromBody] DeleteJourneyPostImageCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { JourneyId = journeyId }, ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }
}
