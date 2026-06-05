using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Notifications.Commands.MarkAllRead;
using Paddokk.Core.Features.Notifications.Commands.MarkNotificationRead;
using Paddokk.Core.Features.Notifications.Queries.GetNotifications;
using Paddokk.Core.Features.Notifications.Queries.GetUnreadCount;
using Paddokk.Core.Models.DTOs.Notification;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/notifications")]
[Authorize]
public class NotificationsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the authenticated user's notifications, newest first")]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
        CancellationToken ct,
        [FromQuery] bool? unread = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetNotificationsQuery(unread, page, pageSize), ct);
        return OkOrError(result);
    }

    [HttpGet("unread-count")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get the authenticated user's unread notification count")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken ct)
    {
        var result = await sender.Send(new GetUnreadCountQuery(), ct);
        return OkOrError(result);
    }

    [HttpPost("{id}/read")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Mark a single notification as read")]
    public async Task<ActionResult> MarkRead(int id, CancellationToken ct)
    {
        var result = await sender.Send(new MarkNotificationReadCommand(id), ct);
        return OkOrError(result);
    }

    [HttpPost("read-all")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Mark all of the authenticated user's notifications as read")]
    public async Task<ActionResult> MarkAllRead(CancellationToken ct)
    {
        var result = await sender.Send(new MarkAllReadCommand(), ct);
        return OkOrError(result);
    }
}
