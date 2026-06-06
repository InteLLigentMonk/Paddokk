using MediatR;
using Paddokk.Core.Features.Follows.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Notifications.EventHandlers;

/// <summary>
/// Creates exactly one <see cref="NotificationType.NewFollower"/> Notification for the followed
/// User when someone Follows them (fan-out N=1, PRD #162). Fires on both create and reactivate of
/// the Follow relationship — re-Following after an Unfollow reads as "X followed me again" (#178).
/// Self-suppression is defensive — the Subscriptions module already rejects self-Follow upstream.
/// The Notification deep-links to the follower's profile (EntityType "User", story 14).
/// </summary>
public sealed class NewFollowerNotificationHandler(INotificationRepository notifications)
    : INotificationHandler<UserFollowed>
{
    public async Task Handle(UserFollowed notification, CancellationToken cancellationToken)
    {
        if (notification.FollowerId == notification.FollowedId)
            return;

        await notifications.CreateAsync(new Notification
        {
            RecipientId = notification.FollowedId,
            ActorId = notification.FollowerId,
            Type = NotificationType.NewFollower,
            EntityType = "User",
            EntityId = notification.FollowerId,
            Read = false,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);
    }
}
