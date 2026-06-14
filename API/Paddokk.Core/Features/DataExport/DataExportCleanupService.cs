using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport;

public sealed class DataExportCleanupService(
    IDataExportRepository repository,
    IDataExportBlobStore blobStore,
    IOptions<DataExportOptions> options,
    ILogger<DataExportCleanupService> logger)
    : IDataExportCleanupService
{
    private readonly DataExportOptions _options = options.Value;

    public async Task<int> RunAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expiredCount = await ExpireReadyAsync(now, ct);
        await ReclaimStuckBuildingAsync(now, ct);
        return expiredCount;
    }

    private async Task<int> ExpireReadyAsync(DateTime now, CancellationToken ct)
    {
        var expired = await repository.GetExpiredReadyAsync(now, ct);
        var count = 0;

        foreach (var request in expired)
        {
            // Isolate each row: a single blob delete or DB error must not abort the whole sweep.
            // A row left untouched is simply retried on the next cycle.
            try
            {
                if (!string.IsNullOrEmpty(request.BlobUrl))
                    await blobStore.DeleteAsync(request.BlobUrl, ct);

                request.Status = DataExportStatus.Expired;
                await repository.UpdateAsync(request, ct);
                count++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to expire data export {RequestId}; will retry next sweep", request.Id);
            }
        }

        if (count > 0)
            logger.LogInformation("Expired {Count} data export(s)", count);

        return count;
    }

    private async Task ReclaimStuckBuildingAsync(DateTime now, CancellationToken ct)
    {
        var threshold = now.AddMinutes(-_options.StuckBuildingThresholdMinutes);
        var stuck = await repository.GetStuckBuildingAsync(threshold, ct);

        foreach (var request in stuck)
        {
            try
            {
                request.Status = DataExportStatus.Failed;
                request.CompletedAt = now;
                await repository.UpdateAsync(request, ct);
                logger.LogWarning("Reclaimed stuck Building data export {RequestId} to Failed", request.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reclaim stuck data export {RequestId}", request.Id);
            }
        }
    }
}
