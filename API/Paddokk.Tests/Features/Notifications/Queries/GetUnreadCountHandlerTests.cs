using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Notifications.Queries.GetUnreadCount;
using Paddokk.Core.Interfaces;

namespace Paddokk.Tests.Features.Notifications.Queries;

public class GetUnreadCountHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetUnreadCountHandler _handler;

    public GetUnreadCountHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new GetUnreadCountHandler(_notifications, _actor);
    }

    [Fact]
    public async Task Handle_ReturnsRepoCountScopedToActor()
    {
        _notifications.GetUnreadCountAsync("user-1", Arg.Any<CancellationToken>()).Returns(7);

        var result = await _handler.Handle(new GetUnreadCountQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);
        await _notifications.Received(1).GetUnreadCountAsync("user-1", Arg.Any<CancellationToken>());
    }
}
