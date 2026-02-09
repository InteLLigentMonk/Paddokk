using API.Extensions;
using API.Models.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JourneysController : ControllerBase
{
    private readonly IJourneyService _journeyService;
    private readonly ILogger<JourneysController> _logger;

    public JourneysController(IJourneyService journeyService, ILogger<JourneysController> logger)
    {
        _journeyService = journeyService;
        _logger = logger;
    }

    /// <summary>
    /// Search and browse journeys with filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> SearchJourneys([FromQuery] JourneySearchRequest request)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var journeys = await _journeyService.SearchJourneysAsync(request, currentUserId);
        return Ok(journeys);
    }

    /// <summary>
    /// Get specific journey details
    /// </summary>
    [HttpGet("{journeyId}")]
    public async Task<ActionResult<JourneyDto>> GetJourney(int journeyId)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, currentUserId);

        if (journey == null)
            return NotFound(new { message = "Journey not found" });

        return Ok(journey);
    }

    /// <summary>
    /// Get featured journeys (curated/most liked)
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetFeaturedJourneys()
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var journeys = await _journeyService.GetFeaturedJourneysAsync(currentUserId);
        return Ok(journeys);
    }

    /// <summary>
    /// Get trending journeys (recently active)
    /// </summary>
    [HttpGet("trending")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetTrendingJourneys()
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var journeys = await _journeyService.GetTrendingJourneysAsync(currentUserId);
        return Ok(journeys);
    }

    /// <summary>
    /// Subscribe to journey for updates
    /// </summary>
    [HttpPost("{journeyId}/subscribe")]
    [Authorize]
    public async Task<IActionResult> SubscribeToJourney(int journeyId)
    {
        var userId = User.GetUserId();

        // Prevent users from subscribing to their own journeys
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, userId);
        if (journey == null)
            return NotFound(new { message = "Journey not found" });

        if (journey.UserId == userId)
            return BadRequest(new { message = "Cannot subscribe to your own journey" });

        var result = await _journeyService.SubscribeToJourneyAsync(userId, journeyId);

        if (!result)
            return BadRequest(new { message = "Failed to subscribe to journey" });

        return Ok(new { message = "Successfully subscribed to journey" });
    }

    /// <summary>
    /// Unsubscribe from journey
    /// </summary>
    [HttpDelete("{journeyId}/subscribe")]
    [Authorize]
    public async Task<IActionResult> UnsubscribeFromJourney(int journeyId)
    {
        var userId = User.GetUserId();
        var result = await _journeyService.UnsubscribeFromJourneyAsync(userId, journeyId);

        if (!result)
            return BadRequest(new { message = "Failed to unsubscribe from journey" });

        return Ok(new { message = "Successfully unsubscribed from journey" });
    }

    /// <summary>
    /// Like a journey
    /// </summary>
    [HttpPost("{journeyId}/like")]
    [Authorize]
    public async Task<IActionResult> LikeJourney(int journeyId)
    {
        var userId = User.GetUserId();

        // Allow users to like their own journeys
        var result = await _journeyService.LikeJourneyAsync(userId, journeyId);

        if (!result)
            return BadRequest(new { message = "Failed to like journey" });

        return Ok(new { message = "Successfully liked journey" });
    }

    /// <summary>
    /// Unlike a journey
    /// </summary>
    [HttpDelete("{journeyId}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikeJourney(int journeyId)
    {
        var userId = User.GetUserId();
        var result = await _journeyService.UnlikeJourneyAsync(userId, journeyId);

        if (!result)
            return BadRequest(new { message = "Failed to unlike journey" });

        return Ok(new { message = "Successfully unliked journey" });
    }

    /// <summary>
    /// Get journey posts (timeline)
    /// </summary>
    [HttpGet("{journeyId}/posts")]
    public async Task<ActionResult<IEnumerable<JourneyPostDto>>> GetJourneyPosts(
        int journeyId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        // Validate journey exists
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId);
        if (journey == null)
            return NotFound(new { message = "Journey not found" });

        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var posts = await _journeyService.GetJourneyPostsAsync(journeyId, skip, take, currentUserId);
        return Ok(posts);
    }

    /// <summary>
    /// Create new post in journey
    /// </summary>
    [HttpPost("{journeyId}/posts")]
    [Authorize]
    public async Task<ActionResult<JourneyPostDto>> CreateJourneyPost(int journeyId, [FromBody] CreateJourneyPostRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var post = await _journeyService.CreateJourneyPostAsync(userId, journeyId, request);
            return CreatedAtAction(nameof(GetJourneyPost), new { postId = post.Id }, post);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get specific journey post
    /// </summary>
    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<JourneyPostDto>> GetJourneyPost(int postId)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var post = await _journeyService.GetJourneyPostByIdAsync(postId, currentUserId);

        if (post == null)
            return NotFound(new { message = "Post not found" });

        return Ok(post);
    }

    /// <summary>
    /// Update journey post (text content only)
    /// </summary>
    [HttpPut("posts/{postId}")]
    [Authorize]
    public async Task<ActionResult<JourneyPostDto>> UpdateJourneyPost(int postId, [FromBody] UpdateJourneyPostRequest request)
    {
        var userId = User.GetUserId();
        var post = await _journeyService.UpdateJourneyPostAsync(userId, postId, request);

        if (post == null)
            return NotFound(new { message = "Post not found or you don't have permission to edit it" });

        return Ok(post);
    }

    /// <summary>
    /// Delete journey post
    /// </summary>
    [HttpDelete("posts/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteJourneyPost(int postId)
    {
        var userId = User.GetUserId();
        var result = await _journeyService.DeleteJourneyPostAsync(userId, postId);

        if (!result)
            return NotFound(new { message = "Post not found or you don't have permission to delete it" });

        return NoContent();
    }
}
