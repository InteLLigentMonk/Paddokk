using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport;

public sealed class DataExportCleanupService(
    IDataExportRepository repository,
    IDataExportBlobStore blobStore,
    ILogger<DataExportCleanupService> logger)
    : IDataExportCleanupService
{
    public async Task<int> RunAsync(CancellationToken ct)
    {
        var expired = await repository.GetExpiredReadyAsync(DateTime.UtcNow, ct);
        if (expired.Count == 0)
            return 0;

        foreach (var request in expired)
        {
            if (!string.IsNullOrEmpty(request.BlobUrl))
                await blobStore.DeleteAsync(request.BlobUrl, ct);

            request.Status = DataExportStatus.Expired;
            await repository.UpdateAsync(request, ct);
        }

        logger.LogInformation("Expired {Count} data export(s)", expired.Count);
        return expired.Count;
    }
}
