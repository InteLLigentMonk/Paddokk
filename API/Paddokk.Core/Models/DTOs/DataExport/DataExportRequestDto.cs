using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.DataExport;

/// <summary>
/// Status view of a data export request returned to the requesting user. Deliberately omits the
/// blob/SAS URL — the download link is delivered out-of-band by email, never echoed through the API.
/// </summary>
public record DataExportRequestDto(
    Guid Id,
    DataExportStatus Status,
    DateTime RequestedAt,
    DateTime? CompletedAt,
    DateTime? ExpiresAt);
