using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Features.Notifications.EventHandlers;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.EventHandlers;

public class CommentOnYourPostNotificationHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly CommentOnYourPostNotificationHandler _handler;

    public CommentOnYourPostNotificationHandlerTests()
    {
        _handler = new CommentOnYourPostNotificationHandler(_notifications);
    }

    [Fact]
    public async Task Handle_OtherUserCommented_CreatesOneNotificationForPostAuthor()
    {
        var evt = new CommentedOnPost(ActorId: "visitor-1", PostId: 42, PostAuthorId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(1).CreateAsync(
            Arg.Is<Notification>(n =>
                n.RecipientId == "owner-1" &&
                n.ActorId == "visitor-1" &&
                n.Type == NotificationType.CommentOnYourPost &&
                n.EntityType == "JourneyPost" &&
                n.EntityId == "42" &&
                n.Read == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SelfComment_CreatesNoNotification()
    {
        var evt = new CommentedOnPost(ActorId: "owner-1", PostId: 42, PostAuthorId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.DidNotReceive().CreateAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }
}
