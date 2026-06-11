using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.DataExport;

namespace Paddokk.Core.Features.DataExport.Queries.GetDataExportStatus;

public sealed class GetDataExportStatusHandler(IDataExportRepository repository, IActorResolver actor)
    : IRequestHandler<GetDataExportStatusQuery, Result<DataExportRequestDto>>
{
    public async Task<Result<DataExportRequestDto>> Handle(GetDataExportStatusQuery query, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(query.Id, ct);

        // Hide other users' requests as NotFound so ids cannot be probed across tenants.
        if (request is null || request.UserId != actor.UserId)
            return Result<DataExportRequestDto>.Failure(Error.NotFound($"Data export request {query.Id} not found"));

        return Result<DataExportRequestDto>.Success(DataExportMapping.ToDto(request));
    }
}
