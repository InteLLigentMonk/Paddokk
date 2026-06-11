using Microsoft.Extensions.Options;
using Paddokk.Core.Features.DataExport;

namespace Paddokk.Api.BackgroundServices;

/// <summary>
/// Runs the export expiry cleanup once on startup, then on a fixed interval. Thin host wrapper
/// around the testable <see cref="IDataExportCleanupService"/>.
/// </summary>
public sealed class DataExportCleanupWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<DataExportOptions> options,
    ILogger<DataExportCleanupWorker> logger)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(Math.Max(1, options.Value.CleanupIntervalHours));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var cleanup = scope.ServiceProvider.GetRequiredService<IDataExportCleanupService>();
                await cleanup.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Data export cleanup iteration failed");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
