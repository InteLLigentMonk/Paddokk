using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;

public sealed class MarkExportFailedHandler(IDataExportRepository repository)
    : IRequestHandler<MarkExportFailedCommand, Result>
{
    public async Task<Result> Handle(MarkExportFailedCommand command, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(command.RequestId, ct);
        if (request is null)
            return Result.Failure(Error.NotFound($"Data export request {command.RequestId} not found"));

        // Only fail an in-flight request; terminal requests are left untouched (idempotent).
        if (request.Status is not (DataExportStatus.Pending or DataExportStatus.Building))
            return Result.Success();

        request.Status = DataExportStatus.Failed;
        request.CompletedAt = DateTime.UtcNow;
        await repository.UpdateAsync(request, ct);

        return Result.Success();
    }
}
