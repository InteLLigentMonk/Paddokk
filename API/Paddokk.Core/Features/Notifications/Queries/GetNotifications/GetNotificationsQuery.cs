using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Notification;

namespace Paddokk.Core.Features.Notifications.Queries.GetNotifications;

/// <summary>
/// The authenticated actor's Notifications, newest first. The recipient is resolved from
/// <see cref="IActorResolver"/> — a caller can only ever read their own.
/// </summary>
/// <param name="Unread">null = all, true = unread only, false = read only.</param>
public record GetNotificationsQuery(bool? Unread, int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<Result<PagedResult<NotificationDto>>>;
