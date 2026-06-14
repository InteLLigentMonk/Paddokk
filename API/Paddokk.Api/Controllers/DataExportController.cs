using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.DataExport.Commands.RequestDataExport;
using Paddokk.Core.Features.DataExport.Queries.GetDataExportStatus;
using Paddokk.Core.Models.DTOs.DataExport;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/data-export")]
[Authorize]
public class DataExportController(ISender sender) : ApiControllerBase
{
    [HttpPost("request")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Request a GDPR export of the authenticated user's data")]
    public async Task<ActionResult<DataExportRequestDto>> Request(CancellationToken ct)
    {
        var result = await sender.Send(new RequestDataExportCommand(), ct);
        return OkOrError(result);
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the status of one of the authenticated user's export requests")]
    public async Task<ActionResult<DataExportRequestDto>> GetStatus(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetDataExportStatusQuery(id), ct);
        return OkOrError(result);
    }
}
