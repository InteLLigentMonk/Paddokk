using NSubstitute;
using Paddokk.Core.Features.Follows.Events;
using Paddokk.Core.Features.Notifications.EventHandlers;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.EventHandlers;

public class NewFollowerNotificationHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly NewFollowerNotificationHandler _handler;

    public NewFollowerNotificationHandlerTests()
    {
        _handler = new NewFollowerNotificationHandler(_notifications);
    }

    [Fact]
    public async Task Handle_UserFollowedAnother_CreatesOneNotificationForFollowedUser()
    {
        var evt = new UserFollowed(FollowerId: "follower-1", FollowedId: "followed-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(1).CreateAsync(
            Arg.Is<Notification>(n =>
                n.RecipientId == "followed-1" &&
                n.ActorId == "follower-1" &&
                n.Type == NotificationType.NewFollower &&
                n.EntityType == "User" &&
                n.EntityId == "follower-1" &&
                n.Read == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FollowRaisedTwice_CreatesNotificationEachTime()
    {
        // Re-Following after an Unfollow republishes UserFollowed (reactivate path, #178). Each
        // event reads as "X followed me again", so the handler creates a fresh Notification.
        var evt = new UserFollowed(FollowerId: "follower-1", FollowedId: "followed-1");

        await _handler.Handle(evt, CancellationToken.None);
        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(2).CreateAsync(
            Arg.Is<Notification>(n => n.Type == NotificationType.NewFollower),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SelfFollow_CreatesNoNotification()
    {
        // Defensive: the Subscriptions module already rejects self-Follow upstream with Conflict.
        var evt = new UserFollowed(FollowerId: "user-1", FollowedId: "user-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.DidNotReceive().CreateAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }
}
