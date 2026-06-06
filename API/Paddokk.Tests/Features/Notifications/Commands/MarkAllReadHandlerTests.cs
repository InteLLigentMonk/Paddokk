using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Notifications.Commands.MarkAllRead;
using Paddokk.Core.Interfaces;

namespace Paddokk.Tests.Features.Notifications.Commands;

public class MarkAllReadHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly MarkAllReadHandler _handler;

    public MarkAllReadHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new MarkAllReadHandler(_notifications, _actor);
    }

    [Fact]
    public async Task Handle_MarksAllReadScopedToActorOnly()
    {
        _notifications.MarkAllReadAsync("user-1", Arg.Any<CancellationToken>()).Returns(3);

        var result = await _handler.Handle(new MarkAllReadCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _notifications.Received(1).MarkAllReadAsync("user-1", Arg.Any<CancellationToken>());
        await _notifications.DidNotReceive().MarkAllReadAsync(
            Arg.Is<string>(id => id != "user-1"), Arg.Any<CancellationToken>());
    }
}
