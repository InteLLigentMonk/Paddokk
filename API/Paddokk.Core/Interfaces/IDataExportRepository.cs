using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

/// <summary>
/// Persistence seam for GDPR data export requests (ADR-0001). Handler and service tests mock this
/// interface; the concrete implementation lives in the Data layer over <c>PaddokkDbContext</c>.
/// </summary>
public interface IDataExportRepository
{
    Task AddAsync(DataExportRequest request, CancellationToken cancellationToken);

    Task<DataExportRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>The user's in-flight request (Pending or Building), if any. Used for rate-limiting.</summary>
    Task<DataExportRequest?> GetOutstandingForUserAsync(string userId, CancellationToken cancellationToken);

    /// <summary>The user's most recent terminal request (Ready or Failed), if any. Used for the cooldown check.</summary>
    Task<DataExportRequest?> GetMostRecentCompletedForUserAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically claims the oldest Pending request by transitioning it to Building, returning it
    /// for processing. Returns null when nothing is pending.
    /// </summary>
    Task<DataExportRequest?> ClaimNextPendingAsync(CancellationToken cancellationToken);

    /// <summary>Ready requests whose <c>ExpiresAt</c> is in the past — candidates for cleanup.</summary>
    Task<IReadOnlyList<DataExportRequest>> GetExpiredReadyAsync(DateTime now, CancellationToken cancellationToken);

    /// <summary>
    /// Building requests older than <paramref name="olderThan"/> — treated as stuck (worker died
    /// mid-process) and reclaimed by the cleanup sweep.
    /// </summary>
    Task<IReadOnlyList<DataExportRequest>> GetStuckBuildingAsync(DateTime olderThan, CancellationToken cancellationToken);

    Task UpdateAsync(DataExportRequest request, CancellationToken cancellationToken);
}
