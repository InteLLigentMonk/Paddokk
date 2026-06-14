namespace Paddokk.Core.Models.Entities;

/// <summary>
/// Lifecycle of a GDPR data export. Persisted as text (see DataExportRequestConfiguration) so
/// reordering members can never silently remap existing rows.
/// </summary>
public enum DataExportStatus
{
    // Requested, waiting for the background worker to pick it up.
    Pending,

    // Worker has claimed it and is assembling the export document.
    Building,

    // Export written to blob, SAS link emailed; downloadable until ExpiresAt.
    Ready,

    // Assembly or upload failed; the user may request a fresh export after the cooldown.
    Failed,

    // Past ExpiresAt; the blob has been deleted and the link no longer works.
    Expired
}
