using System.Text.RegularExpressions;

namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Builds the per-user blob name for an export. Validates the user id before it becomes a path
/// segment so a malformed id can never escape its folder or land the blob at an attacker-influenced
/// path (the same id is later split back out for cleanup).
/// </summary>
public static partial class DataExportBlobNaming
{
    [GeneratedRegex("^[A-Za-z0-9_-]+$")]
    private static partial Regex SafeSegment();

    public static string BuildBlobName(string userId, Guid requestId)
    {
        if (string.IsNullOrEmpty(userId) || !SafeSegment().IsMatch(userId))
            throw new ArgumentException($"User id is not a valid blob path segment: '{userId}'", nameof(userId));

        return $"{userId}/{requestId}.json";
    }
}
