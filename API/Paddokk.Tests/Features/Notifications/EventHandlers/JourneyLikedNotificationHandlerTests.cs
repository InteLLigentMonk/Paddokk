using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Events;
using Paddokk.Core.Features.Notifications.EventHandlers;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.EventHandlers;

public class JourneyLikedNotificationHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly JourneyLikedNotificationHandler _handler;

    public JourneyLikedNotificationHandlerTests()
    {
        _handler = new JourneyLikedNotificationHandler(_notifications);
    }

    [Fact]
    public async Task Handle_OtherUserLiked_CreatesOneNotificationForOwner()
    {
        var evt = new JourneyLiked(ActorId: "visitor-1", JourneyId: 42, JourneyOwnerId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(1).CreateAsync(
            Arg.Is<Notification>(n =>
                n.RecipientId == "owner-1" &&
                n.ActorId == "visitor-1" &&
                n.Type == NotificationType.JourneyLiked &&
                n.EntityType == "Journey" &&
                n.EntityId == "42" &&
                n.Read == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SelfLike_CreatesNoNotification()
    {
        var evt = new JourneyLiked(ActorId: "owner-1", JourneyId: 42, JourneyOwnerId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.DidNotReceive().CreateAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }
}
