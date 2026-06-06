using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Events;
using Paddokk.Core.Features.Notifications.EventHandlers;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.EventHandlers;

public class CarLikedNotificationHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly CarLikedNotificationHandler _handler;

    public CarLikedNotificationHandlerTests()
    {
        _handler = new CarLikedNotificationHandler(_notifications);
    }

    [Fact]
    public async Task Handle_OtherUserLiked_CreatesOneNotificationForOwner()
    {
        var evt = new CarLiked(ActorId: "visitor-1", UserCarId: 42, CarOwnerId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(1).CreateAsync(
            Arg.Is<Notification>(n =>
                n.RecipientId == "owner-1" &&
                n.ActorId == "visitor-1" &&
                n.Type == NotificationType.CarLiked &&
                n.EntityType == "UserCar" &&
                n.EntityId == "42" &&
                n.Read == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SelfLike_CreatesNoNotification()
    {
        var evt = new CarLiked(ActorId: "owner-1", UserCarId: 42, CarOwnerId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.DidNotReceive().CreateAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }
}
