using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Notification;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class NotificationRepository(PaddokkDbContext db) : INotificationRepository
{
    private readonly PaddokkDbContext _db = db;

    public async Task CreateAsync(Notification notification, CancellationToken cancellationToken)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<NotificationDto> Items, int TotalCount)> GetPagedAsync(
        string recipientId, bool? unread, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        var query = _db.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientId == recipientId);

        if (unread is not null)
            query = query.Where(n => n.Read != unread.Value);

        var ordered = query.OrderByDescending(n => n.CreatedAt);

        var total = await ordered.CountAsync(cancellationToken);

        // Actor fields are joined here (ADR-0003): the row stores only ActorId. TargetUrl is filled
        // in a second pass so per-type link resolution stays out of the SQL projection.
        var items = await ordered
            .Skip((p - 1) * s)
            .Take(s)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                EntityType = n.EntityType,
                EntityId = n.EntityId,
                TargetUrl = string.Empty,
                Read = n.Read,
                CreatedAt = n.CreatedAt,
                ActorId = n.ActorId,
                ActorUsername = n.Actor.Username,
                ActorDisplayName = n.Actor.DisplayName,
                ActorAvatarUrl = n.Actor.AvatarUrl,
            })
            .ToListAsync(cancellationToken);

        var resolved = await ResolveTargetUrlsAsync(items, cancellationToken);

        return (resolved, total);
    }

    public Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken)
    {
        // Predicate matches the partial index IX_Notifications_RecipientId_Unread.
        return _db.Notifications
            .Where(n => n.RecipientId == recipientId && n.Read == false)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> MarkReadAsync(int id, string recipientId, CancellationToken cancellationToken)
    {
        // Scoped to the recipient so a caller can never mark another user's row. Setting an
        // already-read row stays a matched update, so the operation is idempotent.
        var affected = await _db.Notifications
            .Where(n => n.Id == id && n.RecipientId == recipientId)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Read, true), cancellationToken);

        return affected > 0;
    }

    public Task<int> MarkAllReadAsync(string recipientId, CancellationToken cancellationToken)
    {
        return _db.Notifications
            .Where(n => n.RecipientId == recipientId && n.Read == false)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Read, true), cancellationToken);
    }

    /// <summary>
    /// Fills <see cref="NotificationDto.TargetUrl"/> per <c>EntityType</c> using batch lookups so
    /// adding a future type means adding a branch here, not an N+1. Unresolvable rows keep an empty
    /// target — the navigation surface renders its own "no longer available" state (story 14).
    /// </summary>
    private async Task<IReadOnlyList<NotificationDto>> ResolveTargetUrlsAsync(
        IReadOnlyList<NotificationDto> items, CancellationToken cancellationToken)
    {
        var journeyTargets = await ResolveJourneyTargetsAsync(items, cancellationToken);

        return items
            .Select(item => item.EntityType switch
            {
                "Journey" when journeyTargets.TryGetValue(item.EntityId, out var url) => item with { TargetUrl = url },
                _ => item,
            })
            .ToList();
    }

    private async Task<IReadOnlyDictionary<string, string>> ResolveJourneyTargetsAsync(
        IReadOnlyList<NotificationDto> items, CancellationToken cancellationToken)
    {
        var ids = items
            .Where(i => i.EntityType == "Journey")
            .Select(i => int.TryParse(i.EntityId, out var id) ? id : (int?)null)
            .Where(id => id is not null)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return new Dictionary<string, string>();

        var rows = await _db.Journeys
            .AsNoTracking()
            .Where(j => ids.Contains(j.Id))
            .Select(j => new { j.Id, j.Slug, OwnerUsername = j.User.Username })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(
            r => r.Id.ToString(),
            r => $"/users/{r.OwnerUsername}/journeys/{r.Slug}");
    }
}
