using MediatR;
using Paddokk.Core.Features.Journeys.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Notifications.EventHandlers;

/// <summary>
/// Creates exactly one <see cref="NotificationType.JourneyLiked"/> Notification for the Journey
/// owner when their Journey is Liked (fan-out N=1, PRD #162). Self-suppression is defensive — the
/// producer already rejects self-likes — so the bell never echoes a User's own action (story 13).
/// </summary>
public sealed class JourneyLikedNotificationHandler(INotificationRepository notifications)
    : INotificationHandler<JourneyLiked>
{
    public async Task Handle(JourneyLiked notification, CancellationToken cancellationToken)
    {
        if (notification.JourneyOwnerId == notification.ActorId)
            return;

        await notifications.CreateAsync(new Notification
        {
            RecipientId = notification.JourneyOwnerId,
            ActorId = notification.ActorId,
            Type = NotificationType.JourneyLiked,
            EntityType = "Journey",
            EntityId = notification.JourneyId.ToString(),
            Read = false,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);
    }
}
