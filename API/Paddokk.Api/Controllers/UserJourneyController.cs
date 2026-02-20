using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/journeys")]
[Authorize]
public class UserJourneysController : ControllerBase
{
    private readonly IJourneyService _journeyService;
    private readonly ILogger<UserJourneysController> _logger;

    public UserJourneysController(IJourneyService journeyService, ILogger<UserJourneysController> logger)
    {
        _journeyService = journeyService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's journeys
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetUserJourneys()
    {
        var userId = User.GetUserId();
        var journeys = await _journeyService.GetUserJourneysAsync(userId, userId);
        return Ok(journeys);
    }

    /// <summary>
    /// Get specific user journey
    /// </summary>
    [HttpGet("{journeyId}")]
    public async Task<ActionResult<JourneyDto>> GetUserJourney(int journeyId)
    {
        var userId = User.GetUserId();
        var journey = await _journeyService.GetJourneyByIdAsync(journeyId, userId);

        if (journey == null || journey.UserId != userId)
            return NotFound(new { message = "Journey not found or you don't own it" });

        return Ok(journey);
    }

    /// <summary>
    /// Create new journey
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<JourneyDto>> CreateJourney([FromBody] CreateJourneyRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var journey = await _journeyService.CreateJourneyAsync(userId, request);
            return CreatedAtAction(nameof(GetUserJourney), new { journeyId = journey.Id }, journey);
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
    /// Update journey details
    /// </summary>
    [HttpPut("{journeyId}")]
    public async Task<ActionResult<JourneyDto>> UpdateJourney(int journeyId, [FromBody] UpdateJourneyRequest request)
    {
        var userId = User.GetUserId();
        var journey = await _journeyService.UpdateJourneyAsync(userId, journeyId, request);

        if (journey == null)
            return NotFound(new { message = "Journey not found or you don't own it" });

        return Ok(journey);
    }

    /// <summary>
    /// Delete journey
    /// </summary>
    [HttpDelete("{journeyId}")]
    public async Task<IActionResult> DeleteJourney(int journeyId)
    {
        var userId = User.GetUserId();
        var result = await _journeyService.DeleteJourneyAsync(userId, journeyId);

        if (!result)
            return NotFound(new { message = "Journey not found or you don't own it" });

        return NoContent();
    }

    /// <summary>
    /// Get user's default active journey (for smart FAB)
    /// </summary>
    [HttpGet("default-active")]
    public async Task<ActionResult<JourneyDto>> GetDefaultActiveJourney()
    {
        var userId = User.GetUserId();
        var journey = await _journeyService.GetUserDefaultActiveJourneyAsync(userId);

        if (journey == null)
            return NotFound(new { message = "No default active journey set" });

        return Ok(journey);
    }

    /// <summary>
    /// Set journey as default active (for smart FAB)
    /// </summary>
    [HttpPut("{journeyId}/set-default-active")]
    public async Task<IActionResult> SetDefaultActiveJourney(int journeyId)
    {
        var userId = User.GetUserId();
        var result = await _journeyService.SetUserDefaultActiveJourneyAsync(userId, journeyId);

        if (!result)
            return BadRequest(new { message = "Failed to set default active journey. Journey not found or not owned by user." });

        return Ok(new { message = "Default active journey updated successfully" });
    }

    /// <summary>
    /// Get user's journey statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<JourneyStatsDto>> GetUserJourneyStats()
    {
        var userId = User.GetUserId();
        var stats = await _journeyService.GetUserJourneyStatsAsync(userId);
        return Ok(stats);
    }

    /// <summary>
    /// Check if user can create more journeys
    /// </summary>
    [HttpGet("can-create")]
    public async Task<ActionResult<object>> CanCreateJourney()
    {
        var userId = User.GetUserId();
        var canCreate = await _journeyService.CanUserCreateJourneyAsync(userId);

        // Get current journey count and limits
        var subscriptionTier = User.GetSubscriptionTier();
        var maxJourneys = subscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        var currentCount = await _journeyService.GetUserJourneyStatsAsync(userId);

        return Ok(new
        {
            canCreate,
            currentCount = currentCount.TotalJourneys,
            maxJourneys = maxJourneys == int.MaxValue ? "Unlimited" : maxJourneys.ToString(),
            subscriptionTier = subscriptionTier.ToString()
        });
    }
}
