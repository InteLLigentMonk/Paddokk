using MediatR;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Notifications.EventHandlers;

/// <summary>
/// Creates exactly one <see cref="NotificationType.ReplyToYourComment"/> Notification for the
/// author of the parent Comment when the JourneyPost owner Replies to it (fan-out N=1, PRD #162).
/// Self-suppression is defensive — the parent Comment author would only equal the actor when the
/// JourneyPost owner Replies to their own Comment — so the bell never echoes a User's own action.
/// The Notification deep-links to the owning JourneyPost (EntityType "JourneyPost", story 14).
/// </summary>
public sealed class ReplyToYourCommentNotificationHandler(INotificationRepository notifications)
    : INotificationHandler<RepliedToComment>
{
    public async Task Handle(RepliedToComment notification, CancellationToken cancellationToken)
    {
        if (notification.ParentCommentAuthorId == notification.ActorId)
            return;

        await notifications.CreateAsync(new Notification
        {
            RecipientId = notification.ParentCommentAuthorId,
            ActorId = notification.ActorId,
            Type = NotificationType.ReplyToYourComment,
            EntityType = "JourneyPost",
            EntityId = notification.PostId.ToString(),
            Read = false,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);
    }
}
