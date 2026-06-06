using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Commands.MarkAllRead;

public sealed class MarkAllReadHandler(INotificationRepository notifications, IActorResolver actor)
    : IRequestHandler<MarkAllReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllReadCommand command, CancellationToken ct)
    {
        await notifications.MarkAllReadAsync(actor.UserId, ct);
        return Result.Success();
    }
}
