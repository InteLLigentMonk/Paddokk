namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Processes a single pending export end-to-end: claim -> assemble -> upload -> mark complete (or
/// mark failed on error). Invoked repeatedly by the background worker.
/// </summary>
public interface IDataExportProcessor
{
    /// <summary>Processes the next pending request. Returns false when nothing is pending.</summary>
    Task<bool> ProcessNextAsync(CancellationToken cancellationToken);
}
