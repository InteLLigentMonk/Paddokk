using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Notifications.Commands.MarkNotificationRead;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Tests.Features.Notifications.Commands;

public class MarkNotificationReadHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly MarkNotificationReadHandler _handler;

    public MarkNotificationReadHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new MarkNotificationReadHandler(_notifications, _actor);
    }

    [Fact]
    public async Task Handle_OwnedRow_MarksReadScopedToActor()
    {
        _notifications.MarkReadAsync(5, "user-1", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new MarkNotificationReadCommand(5), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _notifications.Received(1).MarkReadAsync(5, "user-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyRead_IsIdempotentSuccess()
    {
        // Repo treats an already-read owned row as a matched update (returns true).
        _notifications.MarkReadAsync(5, "user-1", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new MarkNotificationReadCommand(5), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MissingOrOtherUsersRow_ReturnsNotFound()
    {
        _notifications.MarkReadAsync(5, "user-1", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new MarkNotificationReadCommand(5), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
