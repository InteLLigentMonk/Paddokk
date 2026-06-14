using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class DataExportRepository(PaddokkDbContext db) : IDataExportRepository
{
    private readonly PaddokkDbContext _db = db;

    public async Task AddAsync(DataExportRequest request, CancellationToken ct)
    {
        _db.DataExportRequests.Add(request);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<DataExportRequest?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.DataExportRequests.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<DataExportRequest?> GetOutstandingForUserAsync(string userId, CancellationToken ct)
    {
        return await _db.DataExportRequests
            .Where(r => r.UserId == userId &&
                        (r.Status == DataExportStatus.Pending || r.Status == DataExportStatus.Building))
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DataExportRequest?> GetMostRecentCompletedForUserAsync(string userId, CancellationToken ct)
    {
        return await _db.DataExportRequests
            .Where(r => r.UserId == userId &&
                        (r.Status == DataExportStatus.Ready || r.Status == DataExportStatus.Failed))
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DataExportRequest?> ClaimNextPendingAsync(CancellationToken ct)
    {
        var request = await _db.DataExportRequests
            .Where(r => r.Status == DataExportStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .FirstOrDefaultAsync(ct);

        if (request is null)
            return null;

        request.Status = DataExportStatus.Building;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another worker claimed this row first (xmin concurrency token). Leave it to them and
            // let the caller poll again rather than double-processing.
            return null;
        }

        return request;
    }

    public async Task<IReadOnlyList<DataExportRequest>> GetExpiredReadyAsync(DateTime now, CancellationToken ct)
    {
        return await _db.DataExportRequests
            .Where(r => r.Status == DataExportStatus.Ready && r.ExpiresAt != null && r.ExpiresAt < now)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DataExportRequest>> GetStuckBuildingAsync(DateTime olderThan, CancellationToken ct)
    {
        return await _db.DataExportRequests
            .Where(r => r.Status == DataExportStatus.Building && r.RequestedAt < olderThan)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(DataExportRequest request, CancellationToken ct)
    {
        _db.DataExportRequests.Update(request);
        await _db.SaveChangesAsync(ct);
    }
}
