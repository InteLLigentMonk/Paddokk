using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JourneysController : ControllerBase
{
    private readonly IJourneyService _journeyService;

    public JourneysController(IJourneyService journeyService)
    {
        _journeyService = journeyService;
    }

    /// <summary>
    /// Search and browse journeys with filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> SearchJourneys([FromQuery] JourneySearchRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(await _journeyService.SearchJourneysAsync(request, cancellationToken, currentUserId));
    }

    /// <summary>
    /// Get specific journey details
    /// </summary>
    [HttpGet("{journeyId}")]
    public async Task<ActionResult<JourneyDto>> GetJourney(int journeyId, CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, cancellationToken, currentUserId);

        if (journey is null)
            return NotFound();

        return Ok(journey);
    }

    /// <summary>
    /// Get featured journeys (curated/most liked)
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetFeaturedJourneys(CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(await _journeyService.GetFeaturedJourneysAsync(cancellationToken, currentUserId));
    }

    /// <summary>
    /// Get trending journeys (recently active)
    /// </summary>
    [HttpGet("trending")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetTrendingJourneys(CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(await _journeyService.GetTrendingJourneysAsync(cancellationToken, currentUserId));
    }

    /// <summary>
    /// Subscribe to journey for updates
    /// </summary>
    [HttpPost("{journeyId}/subscribe")]
    [Authorize]
    public async Task<IActionResult> SubscribeToJourney(int journeyId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, cancellationToken, userId);

        if (journey is null)
            return NotFound();

        if (journey.UserId == userId)
            throw new InvalidOperationException("Cannot subscribe to your own journey");

        await _journeyService.SubscribeToJourneyAsync(userId, journeyId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Unsubscribe from journey
    /// </summary>
    [HttpDelete("{journeyId}/subscribe")]
    [Authorize]
    public async Task<IActionResult> UnsubscribeFromJourney(int journeyId, CancellationToken cancellationToken)
    {
        await _journeyService.UnsubscribeFromJourneyAsync(User.GetUserId(), journeyId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Like a journey
    /// </summary>
    [HttpPost("{journeyId}/like")]
    [Authorize]
    public async Task<IActionResult> LikeJourney(int journeyId, CancellationToken cancellationToken)
    {
        await _journeyService.LikeJourneyAsync(User.GetUserId(), journeyId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Unlike a journey
    /// </summary>
    [HttpDelete("{journeyId}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikeJourney(int journeyId, CancellationToken cancellationToken)
    {
        await _journeyService.UnlikeJourneyAsync(User.GetUserId(), journeyId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get journey posts (timeline)
    /// </summary>
    [HttpGet("{journeyId}/posts")]
    public async Task<ActionResult<IEnumerable<JourneyPostDto>>> GetJourneyPosts(
        int journeyId, CancellationToken cancellationToken,
        [FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, cancellationToken);
        if (journey is null)
            return NotFound();

        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(await _journeyService.GetJourneyPostsAsync(journeyId, cancellationToken, skip, take, currentUserId));
    }

    /// <summary>
    /// Create new post in journey
    /// </summary>
    [HttpPost("{journeyId}/posts")]
    [Authorize]
    public async Task<ActionResult<JourneyPostDto>> CreateJourneyPost(int journeyId, [FromBody] CreateJourneyPostRequest request, CancellationToken cancellationToken)
    {
        var post = await _journeyService.CreateJourneyPostAsync(User.GetUserId(), journeyId, request, cancellationToken);
        return CreatedAtAction(nameof(GetJourneyPost), new { postId = post.Id }, post);
    }

    /// <summary>
    /// Get specific journey post
    /// </summary>
    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<JourneyPostDto>> GetJourneyPost(int postId, CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var post = await _journeyService.GetJourneyPostByIdAsync(postId, cancellationToken, currentUserId);

        if (post is null)
            return NotFound();

        return Ok(post);
    }

    /// <summary>
    /// Update journey post (text content only)
    /// </summary>
    [HttpPut("posts/{postId}")]
    [Authorize]
    public async Task<ActionResult<JourneyPostDto>> UpdateJourneyPost(int postId, [FromBody] UpdateJourneyPostRequest request, CancellationToken cancellationToken)
    {
        var post = await _journeyService.UpdateJourneyPostAsync(User.GetUserId(), postId, request, cancellationToken);

        if (post is null)
            return NotFound();

        return Ok(post);
    }

    /// <summary>
    /// Delete journey post
    /// </summary>
    [HttpDelete("posts/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteJourneyPost(int postId, CancellationToken cancellationToken)
    {
        var result = await _journeyService.DeleteJourneyPostAsync(User.GetUserId(), postId, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
