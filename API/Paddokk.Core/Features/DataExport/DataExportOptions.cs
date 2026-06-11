namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Configuration for the data export pipeline. Bound from the "DataExport" configuration section;
/// every value has a safe default so the feature works without explicit configuration.
/// </summary>
public class DataExportOptions
{
    public const string SectionName = "DataExport";

    // How long a Ready export (and its blob/SAS link) stays valid.
    public int ExportTtlDays { get; set; } = 7;

    // How long after a completed/failed export the user must wait before requesting another.
    public int CooldownHours { get; set; } = 24;

    // Private Azure Blob container the export JSON is written to.
    public string BlobContainerName { get; set; } = "data-exports";

    // How often the background worker polls for Pending requests.
    public int PollIntervalSeconds { get; set; } = 30;

    // How often the expiry cleanup sweep runs (it also runs once on host startup).
    public int CleanupIntervalHours { get; set; } = 24;
}
