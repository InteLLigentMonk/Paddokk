using MediatR;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;

public sealed class MarkExportCompleteHandler(
    IDataExportRepository repository,
    IUserRepository userRepository,
    IExportEmailSender emailSender,
    ILogger<MarkExportCompleteHandler> logger)
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
        // Persist only the unsigned blob URL — the SAS query string is a bearer credential and must
        // not be stored at rest. Cleanup needs the path, not the signature.
        request.BlobUrl = command.BlobUri;
        request.ExpiresAt = command.ExpiresAt;
        request.CompletedAt = DateTime.UtcNow;
        await repository.UpdateAsync(request, ct);

        // Email runs after the Ready transition is persisted and is best-effort: a Resend outage must
        // not flip the export back to Failed or orphan the blob that was already written.
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId, ct);
            if (!string.IsNullOrEmpty(user?.Email))
                await emailSender.SendExportReadyAsync(user.Email, command.DownloadUrl, command.ExpiresAt, ct);
            else
                logger.LogWarning("Data export {RequestId} ready but user {UserId} has no email on file",
                    request.Id, request.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send data export ready email for request {RequestId}", request.Id);
        }

        return Result.Success();
    }
}
