using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Feed.Queries.GetFeed;
using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize]
public class FeedController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the authenticated user's personalised, strictly chronological feed")]
    public async Task<ActionResult<PagedResult<FeedItemDto>>> GetFeed(
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetFeedQuery(page, pageSize), ct);
        return OkOrError(result);
    }
}
