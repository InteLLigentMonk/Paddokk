using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.Dashboard;
using Paddokk.Core.Features.Journeys.Queries.GetTrendingJourneys;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/users/me/[controller]")]
[Authorize]
public class DashboardController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get dashboard data for authenticated user")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(CancellationToken ct)
    {
        var result = await sender.Send(new GetDashboardQuery(), ct);
        return OkOrError(result);
    }

    [HttpGet("feed")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get activity feed for dashboard (trending journeys)")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetActivityFeed(
        CancellationToken ct,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var feed = await sender.Send(new GetTrendingJourneysQuery(), ct);
        return Ok(feed.Skip(skip).Take(take));
    }
}
