using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadHandler(INotificationRepository notifications, IActorResolver actor)
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationReadCommand command, CancellationToken ct)
    {
        // Scoped to the actor: another user's id is indistinguishable from a missing row, so both
        // surface as NotFound rather than leaking existence.
        var found = await notifications.MarkReadAsync(command.Id, actor.UserId, ct);

        return found
            ? Result.Success()
            : Result.Failure(Error.NotFound($"Notification {command.Id} not found"));
    }
}
