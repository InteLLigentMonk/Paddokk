using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Notification;

namespace Paddokk.Core.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsHandler(INotificationRepository notifications, IActorResolver actor)
    : IRequestHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    public async Task<Result<PagedResult<NotificationDto>>> Handle(GetNotificationsQuery query, CancellationToken ct)
    {
        var (items, total) = await notifications.GetPagedAsync(actor.UserId, query.Unread, query.Page, query.PageSize, ct);

        return Result<PagedResult<NotificationDto>>.Success(
            PagedResult<NotificationDto>.Create(items, total, query.Page, query.PageSize));
    }
}
