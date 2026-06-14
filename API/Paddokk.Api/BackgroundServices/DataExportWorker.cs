using Microsoft.Extensions.Options;
using Paddokk.Core.Features.DataExport;

namespace Paddokk.Api.BackgroundServices;

/// <summary>
/// Polls for Pending export requests and processes them. Thin host wrapper: all per-request logic
/// lives in the testable <see cref="IDataExportProcessor"/>; this type only owns the loop, the DI
/// scope (the processor and its repositories are scoped), and error isolation between iterations.
/// </summary>
public sealed class DataExportWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<DataExportOptions> options,
    ILogger<DataExportWorker> logger)
    : BackgroundService
{
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(Math.Max(1, options.Value.PollIntervalSeconds));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IDataExportProcessor>();

                // Drain everything pending before sleeping.
                while (await processor.ProcessNextAsync(stoppingToken))
                {
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Data export worker iteration failed");
            }

            try
            {
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
