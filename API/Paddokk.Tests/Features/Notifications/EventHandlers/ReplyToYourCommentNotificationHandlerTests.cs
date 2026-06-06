using NSubstitute;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Features.Notifications.EventHandlers;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.EventHandlers;

public class ReplyToYourCommentNotificationHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly ReplyToYourCommentNotificationHandler _handler;

    public ReplyToYourCommentNotificationHandlerTests()
    {
        _handler = new ReplyToYourCommentNotificationHandler(_notifications);
    }

    [Fact]
    public async Task Handle_OwnerRepliedToAnotherUsersComment_CreatesOneNotificationForCommentAuthor()
    {
        var evt = new RepliedToComment(ActorId: "owner-1", PostId: 42, ParentCommentAuthorId: "commenter-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.Received(1).CreateAsync(
            Arg.Is<Notification>(n =>
                n.RecipientId == "commenter-1" &&
                n.ActorId == "owner-1" &&
                n.Type == NotificationType.ReplyToYourComment &&
                n.EntityType == "JourneyPost" &&
                n.EntityId == "42" &&
                n.Read == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OwnerRepliedToTheirOwnComment_CreatesNoNotification()
    {
        // Defensive: the parent Comment author equals the actor only when the JourneyPost owner
        // Replies to their own Comment. Self-suppression keeps the bell from echoing (story 13).
        var evt = new RepliedToComment(ActorId: "owner-1", PostId: 42, ParentCommentAuthorId: "owner-1");

        await _handler.Handle(evt, CancellationToken.None);

        await _notifications.DidNotReceive().CreateAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }
}
