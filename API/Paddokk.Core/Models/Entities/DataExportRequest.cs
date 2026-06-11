namespace Paddokk.Core.Models.Entities;

/// <summary>
/// A single GDPR "export my data" request. Deliberately carries no <see cref="ApplicationUser"/>
/// navigation: that keeps it out of the global "!User.IsDeleted" query filter applied in
/// <c>OnModelCreating</c>, so the background worker and expiry cleanup can see every row regardless
/// of user soft-deletion. The owner's email is resolved on demand via the user repository.
/// </summary>
public class DataExportRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // The user the export belongs to. Stored as the bare id (no navigation) on purpose.
    public string UserId { get; set; } = string.Empty;

    public DataExportStatus Status { get; set; } = DataExportStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    // Set when the export reaches a terminal state (Ready or Failed).
    public DateTime? CompletedAt { get; set; }

    // The blob path/URL of the assembled export. Null until Ready.
    public string? BlobUrl { get; set; }

    // When the Ready download (and its blob) expires. Null until Ready.
    public DateTime? ExpiresAt { get; set; }
}
