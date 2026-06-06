using MediatR;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Notifications.EventHandlers;

/// <summary>
/// Creates exactly one <see cref="NotificationType.CommentOnYourPost"/> Notification for the
/// JourneyPost author when someone writes a top-level Comment on their post (fan-out N=1, PRD #162).
/// Self-suppression stops the bell echoing a User's own Comment on their own post (story 13).
/// </summary>
public sealed class CommentOnYourPostNotificationHandler(INotificationRepository notifications)
    : INotificationHandler<CommentedOnPost>
{
    public async Task Handle(CommentedOnPost notification, CancellationToken cancellationToken)
    {
        if (notification.PostAuthorId == notification.ActorId)
            return;

        await notifications.CreateAsync(new Notification
        {
            RecipientId = notification.PostAuthorId,
            ActorId = notification.ActorId,
            Type = NotificationType.CommentOnYourPost,
            EntityType = "JourneyPost",
            EntityId = notification.PostId.ToString(),
            Read = false,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);
    }
}
