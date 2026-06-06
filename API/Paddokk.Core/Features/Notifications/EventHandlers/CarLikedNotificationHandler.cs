using MediatR;
using Paddokk.Core.Features.Cars.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Notifications.EventHandlers;

/// <summary>
/// Creates exactly one <see cref="NotificationType.CarLiked"/> Notification for the UserCar
/// owner when their UserCar is Liked (fan-out N=1, PRD #162). Self-suppression is defensive — the
/// producer already rejects self-likes — so the bell never echoes a User's own action (story 13).
/// </summary>
public sealed class CarLikedNotificationHandler(INotificationRepository notifications)
    : INotificationHandler<CarLiked>
{
    public async Task Handle(CarLiked notification, CancellationToken cancellationToken)
    {
        if (notification.CarOwnerId == notification.ActorId)
            return;

        await notifications.CreateAsync(new Notification
        {
            RecipientId = notification.CarOwnerId,
            ActorId = notification.ActorId,
            Type = NotificationType.CarLiked,
            EntityType = "UserCar",
            EntityId = notification.UserCarId.ToString(),
            Read = false,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);
    }
}
