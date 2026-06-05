using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Notifications.Queries.GetNotifications;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Notification;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.Queries;

public class GetNotificationsHandlerTests
{
    private readonly INotificationRepository _notifications = Substitute.For<INotificationRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetNotificationsHandler _handler;

    public GetNotificationsHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new GetNotificationsHandler(_notifications, _actor);
    }

    [Fact]
    public async Task Handle_ForwardsUnreadFilterAndPagingScopedToActor()
    {
        _notifications.GetPagedAsync("user-1", true, 2, 10, Arg.Any<CancellationToken>())
            .Returns((new List<NotificationDto>(), 0));

        await _handler.Handle(new GetNotificationsQuery(Unread: true, Page: 2, PageSize: 10), CancellationToken.None);

        await _notifications.Received(1).GetPagedAsync("user-1", true, 2, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WrapsItemsInPagedResultWithCorrectMath()
    {
        var items = new List<NotificationDto> { Dto(1), Dto(2) };
        _notifications.GetPagedAsync("user-1", null, 2, 10, Arg.Any<CancellationToken>())
            .Returns((items, 25));

        var result = await _handler.Handle(
            new GetNotificationsQuery(Unread: null, Page: 2, PageSize: 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEquivalentTo(items);
        result.Value.Page.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(25);
        result.Value.HasNextPage.Should().BeTrue(); // 2 * 10 < 25
    }

    [Fact]
    public async Task Handle_LastPage_HasNextPageFalse()
    {
        _notifications.GetPagedAsync("user-1", null, 3, 10, Arg.Any<CancellationToken>())
            .Returns((new List<NotificationDto> { Dto(1) }, 25));

        var result = await _handler.Handle(
            new GetNotificationsQuery(Unread: null, Page: 3, PageSize: 10), CancellationToken.None);

        result.Value!.HasNextPage.Should().BeFalse(); // 3 * 10 >= 25
    }

    private static NotificationDto Dto(int id) => new()
    {
        Id = id,
        Type = NotificationType.JourneyLiked,
        EntityType = "Journey",
        EntityId = id.ToString(),
        TargetUrl = string.Empty,
        Read = false,
        CreatedAt = DateTime.UtcNow,
        ActorId = "actor-1",
        ActorUsername = "alice",
        ActorDisplayName = "Alice",
        ActorAvatarUrl = null,
    };
}
