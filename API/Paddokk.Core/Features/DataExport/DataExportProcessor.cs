using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;
using Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// End-to-end processing of one export request. Claiming the request also transitions it to
/// Building; completion and failure go through MediatR so the idempotent state/email logic lives in
/// the tested handlers.
/// </summary>
public sealed class DataExportProcessor(
    IDataExportRepository repository,
    IDataExportAssembler assembler,
    IDataExportBlobStore blobStore,
    ISender sender,
    IOptions<DataExportOptions> options,
    ILogger<DataExportProcessor> logger)
    : IDataExportProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly DataExportOptions _options = options.Value;

    public async Task<bool> ProcessNextAsync(CancellationToken ct)
    {
        var request = await repository.ClaimNextPendingAsync(ct);
        if (request is null)
            return false;

        DataExportBlobResult? uploaded = null;
        try
        {
            var document = await assembler.BuildAsync(request.UserId, ct);
            var json = JsonSerializer.Serialize(document, JsonOptions);

            var expiresAt = DateTime.UtcNow.AddDays(_options.ExportTtlDays);
            uploaded = await blobStore.SaveAndCreateDownloadUrlAsync(request.UserId, request.Id, json, expiresAt, ct);

            await sender.Send(new MarkExportCompleteCommand(request.Id, uploaded.BlobUri, uploaded.DownloadUrl, expiresAt), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data export {RequestId} failed during processing", request.Id);
            await FailGracefullyAsync(request.Id, uploaded, ct);
        }

        return true;
    }

    /// <summary>
    /// Best-effort failure handling: delete the blob if it was already written (so completion failing
    /// after upload doesn't orphan it), then mark the request Failed. Neither step is allowed to throw
    /// — a failure here must not wedge the worker; stale Building rows are recovered by the cleanup sweep.
    /// </summary>
    private async Task FailGracefullyAsync(Guid requestId, DataExportBlobResult? uploaded, CancellationToken ct)
    {
        if (uploaded is not null)
        {
            try
            {
                await blobStore.DeleteAsync(uploaded.BlobUri, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete orphan export blob for {RequestId}", requestId);
            }
        }

        try
        {
            await sender.Send(new MarkExportFailedCommand(requestId), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to mark export {RequestId} as failed", requestId);
        }
    }
}
