namespace Paddokk.Core.Interfaces;

/// <summary>
/// Result of writing an export blob: the unsigned blob URL (safe to persist, used for cleanup) and
/// the time-limited signed SAS download URL (a bearer credential — emailed to the user, never stored).
/// </summary>
public record DataExportBlobResult(string BlobUri, string DownloadUrl);

/// <summary>
/// Stores assembled export documents in the private <c>data-exports</c> blob container and produces
/// time-limited SAS download links. Abstracted so the worker and cleanup are testable without the
/// Azure SDK.
/// </summary>
public interface IDataExportBlobStore
{
    /// <summary>
    /// Writes <paramref name="json"/> to a per-user blob and returns the unsigned blob URL plus a
    /// read-only SAS URL that expires at <paramref name="expiresAt"/>.
    /// </summary>
    Task<DataExportBlobResult> SaveAndCreateDownloadUrlAsync(
        string userId, Guid requestId, string json, DateTime expiresAt, CancellationToken cancellationToken);

    /// <summary>Deletes the blob behind a previously stored blob URL (any SAS query is ignored).</summary>
    Task DeleteAsync(string blobUri, CancellationToken cancellationToken);
}
