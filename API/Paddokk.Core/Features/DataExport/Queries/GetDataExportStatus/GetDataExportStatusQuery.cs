using MediatR;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.DataExport;

namespace Paddokk.Core.Features.DataExport.Queries.GetDataExportStatus;

/// <summary>
/// Returns the status of one of the actor's own export requests. Requests belonging to other users
/// are reported as NotFound so ids are not enumerable across tenants.
/// </summary>
public sealed record GetDataExportStatusQuery(Guid Id) : IRequest<Result<DataExportRequestDto>>;
