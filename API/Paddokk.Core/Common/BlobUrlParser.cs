namespace Paddokk.Core.Common;

/// <summary>
/// Parses an Azure Blob URL into its container and blob-name parts. Shared by every service that
/// needs to delete a blob from a stored URL, so the (fragile) path-splitting logic lives in one place.
/// Any SAS query string is ignored.
/// </summary>
public static class BlobUrlParser
{
    /// <summary>
    /// Returns the container name and blob name for <paramref name="blobUrl"/>, or null if the URL
    /// has no blob segment (only a container, or unparseable).
    /// </summary>
    public static (string Container, string BlobName)? Parse(string blobUrl)
    {
        if (!Uri.TryCreate(blobUrl, UriKind.Absolute, out var uri))
            return null;

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2)
            return null;

        var container = segments[0];
        var blobName = string.Join('/', segments.Skip(1));
        return (container, blobName);
    }
}
