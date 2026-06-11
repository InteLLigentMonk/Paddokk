namespace Paddokk.Core.Interfaces;

/// <summary>
/// Stores assembled export documents in the private <c>data-exports</c> blob container and produces
/// time-limited SAS download links. Abstracted so the worker and cleanup are testable without the
/// Azure SDK.
/// </summary>
public interface IDataExportBlobStore
{
    /// <summary>
    /// Writes <paramref name="json"/> to a per-user blob and returns a read-only SAS URL that
    /// expires at <paramref name="expiresAt"/>.
    /// </summary>
    Task<string> SaveAndCreateDownloadUrlAsync(
        string userId, Guid requestId, string json, DateTime expiresAt, CancellationToken cancellationToken);

    /// <summary>Deletes the blob behind a previously issued download URL (SAS query is ignored).</summary>
    Task DeleteAsync(string downloadUrl, CancellationToken cancellationToken);
}
