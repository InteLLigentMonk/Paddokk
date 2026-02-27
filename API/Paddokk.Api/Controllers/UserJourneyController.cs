using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Features.Journeys.Commands.CreateJourney;
using Paddokk.Core.Features.Journeys.Commands.DeleteJourney;
using Paddokk.Core.Features.Journeys.Commands.SetDefaultActiveJourney;
using Paddokk.Core.Features.Journeys.Commands.UpdateJourney;
using Paddokk.Core.Features.Journeys.Queries.CanCreateJourney;
using Paddokk.Core.Features.Journeys.Queries.GetDefaultActiveJourney;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyById;
using Paddokk.Core.Features.Journeys.Queries.GetUserJourneys;
using Paddokk.Core.Features.Journeys.Queries.GetUserJourneyStats;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/users/me/journeys")]
[Authorize]
public class UserJourneysController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get current user's journeys")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetUserJourneys(CancellationToken ct)
    {
        var result = await sender.Send(new GetUserJourneysQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{journeyId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get specific user journey")]
    public async Task<ActionResult<JourneyDto>> GetUserJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new GetJourneyByIdQuery(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        if (result.Value!.UserId != User.GetUserId())
            return NotFound(new { message = "Journey not found or you don't own it" });

        return Ok(result.Value);
    }

    [HttpPost]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Create new journey")]
    public async Task<ActionResult<JourneyDto>> CreateJourney([FromBody] CreateJourneyCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return CreatedAtAction(nameof(GetUserJourney), new { journeyId = result.Value!.Id }, result.Value);
    }

    [HttpPut("{journeyId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update journey details")]
    public async Task<ActionResult<JourneyDto>> UpdateJourney(
        int journeyId, [FromBody] UpdateJourneyCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { JourneyId = journeyId }, ct);
        return OkOrError(result);
    }

    [HttpDelete("{journeyId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete journey")]
    public async Task<IActionResult> DeleteJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpGet("default-active")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get user's default active journey")]
    public async Task<ActionResult<JourneyDto>> GetDefaultActiveJourney(CancellationToken ct)
    {
        var result = await sender.Send(new GetDefaultActiveJourneyQuery(), ct);
        return OkOrError(result);
    }

    [HttpPut("{journeyId}/set-default-active")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Set journey as default active")]
    public async Task<IActionResult> SetDefaultActiveJourney(int journeyId, CancellationToken ct)
    {
        var result = await sender.Send(new SetDefaultActiveJourneyCommand(journeyId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return Ok(new { message = "Default active journey updated successfully" });
    }

    [HttpGet("stats")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get user's journey statistics")]
    public async Task<ActionResult<JourneyStatsDto>> GetUserJourneyStats(CancellationToken ct)
    {
        var result = await sender.Send(new GetUserJourneyStatsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("can-create")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Check if user can create more journeys")]
    public async Task<ActionResult<CanCreateJourneyResponse>> CanCreateJourney(CancellationToken ct)
    {
        var result = await sender.Send(new CanCreateJourneyQuery(), ct);
        return Ok(result);
    }
}
