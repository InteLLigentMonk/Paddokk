using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;

public sealed class MarkExportCompleteHandler(
    IDataExportRepository repository,
    IUserRepository userRepository,
    IExportEmailSender emailSender)
    : IRequestHandler<MarkExportCompleteCommand, Result>
{
    public async Task<Result> Handle(MarkExportCompleteCommand command, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(command.RequestId, ct);
        if (request is null)
            return Result.Failure(Error.NotFound($"Data export request {command.RequestId} not found"));

        // Idempotent: only an in-flight request transitions and emails. A re-issued command for an
        // already-terminal request is a silent no-op so the email is never sent twice.
        if (request.Status is not (DataExportStatus.Pending or DataExportStatus.Building))
            return Result.Success();

        request.Status = DataExportStatus.Ready;
        request.BlobUrl = command.DownloadUrl;
        request.ExpiresAt = command.ExpiresAt;
        request.CompletedAt = DateTime.UtcNow;
        await repository.UpdateAsync(request, ct);

        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (!string.IsNullOrEmpty(user?.Email))
            await emailSender.SendExportReadyAsync(user.Email, command.DownloadUrl, command.ExpiresAt, ct);

        return Result.Success();
    }
}
