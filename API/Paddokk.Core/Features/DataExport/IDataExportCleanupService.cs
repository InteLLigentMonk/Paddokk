namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Sweeps Ready exports whose link has expired: deletes the blob and marks the request Expired.
/// </summary>
public interface IDataExportCleanupService
{
    /// <summary>Runs one cleanup sweep. Returns the number of requests expired.</summary>
    Task<int> RunAsync(CancellationToken cancellationToken);
}
