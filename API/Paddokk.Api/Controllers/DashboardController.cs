using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.Dashboard;

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
}
