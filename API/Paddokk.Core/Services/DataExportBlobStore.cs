using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Services;

/// <summary>
/// Azure Blob implementation of <see cref="IDataExportBlobStore"/>. The container is created with
/// <see cref="PublicAccessType.None"/> so exports are never publicly readable — access is only ever
/// granted through a short-lived read SAS token.
/// </summary>
public sealed class DataExportBlobStore(
    BlobServiceClient blobServiceClient,
    IOptions<DataExportOptions> options,
    ILogger<DataExportBlobStore> logger)
    : IDataExportBlobStore
{
    private readonly DataExportOptions _options = options.Value;

    public async Task<string> SaveAndCreateDownloadUrlAsync(
        string userId, Guid requestId, string json, DateTime expiresAt, CancellationToken ct)
    {
        var container = blobServiceClient.GetBlobContainerClient(_options.BlobContainerName);

        // Private container: SAS-token access only, never public.
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blobName = $"{userId}/{requestId}.json";
        var blob = container.GetBlobClient(blobName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
        }, ct);

        if (!blob.CanGenerateSasUri)
        {
            // Without a shared-key credential we cannot mint a SAS; fail loudly rather than hand
            // back a URL that 403s for the user.
            throw new InvalidOperationException(
                "Blob client cannot generate a SAS URI. A shared-key connection string is required for data export downloads.");
        }

        var sasUri = blob.GenerateSasUri(BlobSasPermissions.Read, new DateTimeOffset(expiresAt, TimeSpan.Zero));
        logger.LogInformation("Wrote data export blob {BlobName} for user {UserId}", blobName, userId);

        return sasUri.ToString();
    }

    public async Task DeleteAsync(string downloadUrl, CancellationToken ct)
    {
        // The download URL is a SAS link; the blob path is everything after the container segment,
        // the SAS query string is ignored.
        var uri = new Uri(downloadUrl);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2)
        {
            logger.LogWarning("Could not parse blob path from export URL; skipping delete");
            return;
        }

        var containerName = segments[0];
        var blobName = string.Join('/', segments.Skip(1));

        var container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct);
        logger.LogInformation("Deleted expired data export blob {BlobName}", blobName);
    }
}
