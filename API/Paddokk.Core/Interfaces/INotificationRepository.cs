using Paddokk.Core.Models.DTOs.Notification;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

/// <summary>
/// Persistence seam for the Notifications module (ADR-0001). Read methods join the actor and
/// resolve deep-link targets on read (ADR-0003); they never read denormalized actor columns.
/// </summary>
public interface INotificationRepository
{
    Task CreateAsync(Notification notification, CancellationToken cancellationToken);

    /// <param name="unread">null = all, true = unread only, false = read only.</param>
    Task<(IReadOnlyList<NotificationDto> Items, int TotalCount)> GetPagedAsync(
        string recipientId, bool? unread, int page, int pageSize, CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken);

    /// <summary>Marks one row read, scoped to its recipient. Idempotent. Returns false if no such row.</summary>
    Task<bool> MarkReadAsync(int id, string recipientId, CancellationToken cancellationToken);

    /// <summary>Marks every unread row read for the recipient. Returns the number of rows flipped.</summary>
    Task<int> MarkAllReadAsync(string recipientId, CancellationToken cancellationToken);
}
