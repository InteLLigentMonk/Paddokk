using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Queries.GetUnreadCount;

public sealed class GetUnreadCountHandler(INotificationRepository notifications, IActorResolver actor)
    : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadCountQuery query, CancellationToken ct)
    {
        var count = await notifications.GetUnreadCountAsync(actor.UserId, ct);
        return Result<int>.Success(count);
    }
}
