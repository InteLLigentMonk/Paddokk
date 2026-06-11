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

        try
        {
            var document = await assembler.BuildAsync(request.UserId, ct);
            var json = JsonSerializer.Serialize(document, JsonOptions);

            var expiresAt = DateTime.UtcNow.AddDays(_options.ExportTtlDays);
            var downloadUrl = await blobStore.SaveAndCreateDownloadUrlAsync(
                request.UserId, request.Id, json, expiresAt, ct);

            await sender.Send(new MarkExportCompleteCommand(request.Id, downloadUrl, expiresAt), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data export {RequestId} failed during processing", request.Id);
            await sender.Send(new MarkExportFailedCommand(request.Id), ct);
        }

        return true;
    }
}
