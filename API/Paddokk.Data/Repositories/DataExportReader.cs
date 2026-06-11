using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

/// <summary>
/// Reads a user's own rows for GDPR export. Every query is filtered by the user's id so the export
/// can never include another user's data.
/// </summary>
public class DataExportReader(PaddokkDbContext db) : IDataExportReader
{
    private readonly PaddokkDbContext _db = db;

    public async Task<ApplicationUser?> GetProfileAsync(string userId, CancellationToken ct)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<IReadOnlyList<UserCar>> GetCarsWithImagesAsync(string userId, CancellationToken ct)
    {
        return await _db.UserCars
            .AsNoTracking()
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .Include(c => c.Images)
            .Where(c => c.PrincipalId == userId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Journey>> GetJourneysAsync(string userId, CancellationToken ct)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Where(j => j.PrincipalId == userId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<JourneyPost>> GetJourneyPostsWithImagesAsync(string userId, CancellationToken ct)
    {
        return await _db.JourneyPosts
            .AsNoTracking()
            .Include(p => p.Images)
            .Where(p => p.AuthorId == userId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PostComment>> GetCommentsAsync(string userId, CancellationToken ct)
    {
        return await _db.PostComments
            .AsNoTracking()
            .Where(c => c.AuthorId == userId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserFollow>> GetActiveFollowsAsync(string userId, CancellationToken ct)
    {
        return await _db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId && f.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<int>> GetNotificationIdsAsync(string userId, CancellationToken ct)
    {
        return await _db.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientId == userId)
            .Select(n => n.Id)
            .ToListAsync(ct);
    }
}
