using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Notifications.Queries.GetNotifications;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Notification;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Notifications.Queries;

/// <summary>
/// Proves the read path resolves actor display fields at read time (ADR-0003) rather than
/// snapshotting them, and orders newest-first. There is no DbContext test harness in the suite
/// (ADR-0001), so the join-on-read seam is modelled by <see cref="FakeJoinOnReadRepository"/>:
/// it stores only ActorId on each row and projects actor fields from a mutable user store on
/// every read — exactly the contract the EF repository implements.
/// </summary>
public class NotificationJoinOnReadTests
{
    private readonly FakeJoinOnReadRepository _repo = new();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetNotificationsHandler _handler;

    public NotificationJoinOnReadTests()
    {
        _actor.UserId.Returns("recipient-1");
        _handler = new GetNotificationsHandler(_repo, _actor);
    }

    [Fact]
    public async Task ActorDisplayFields_ReflectCurrentUserStateAfterUpdate()
    {
        _repo.Users["actor-1"] = new ApplicationUser
        {
            Id = "actor-1", Username = "alice", DisplayName = "Alice", AvatarUrl = "alice.png",
        };
        _repo.Add("recipient-1", "actor-1", DateTime.UtcNow);

        var before = await Read();
        before.ActorDisplayName.Should().Be("Alice");
        before.ActorAvatarUrl.Should().Be("alice.png");

        // Upstream rename + avatar change (or anonymization) — no notification row is touched.
        _repo.Users["actor-1"].DisplayName = "Alice Cooper";
        _repo.Users["actor-1"].Username = "alice-c";
        _repo.Users["actor-1"].AvatarUrl = "new.png";

        var after = await Read();
        after.ActorDisplayName.Should().Be("Alice Cooper");
        after.ActorUsername.Should().Be("alice-c");
        after.ActorAvatarUrl.Should().Be("new.png");
    }

    [Fact]
    public async Task Items_AreOrderedNewestFirst()
    {
        _repo.Users["actor-1"] = new ApplicationUser { Id = "actor-1", Username = "a", DisplayName = "A" };
        var now = DateTime.UtcNow;
        _repo.Add("recipient-1", "actor-1", now.AddMinutes(-10)); // id 1, oldest
        _repo.Add("recipient-1", "actor-1", now);                 // id 2, newest
        _repo.Add("recipient-1", "actor-1", now.AddMinutes(-5));  // id 3, middle

        var (items, _) = await _repo.GetPagedAsync("recipient-1", null, 1, 20, CancellationToken.None);

        items.Select(i => i.Id).Should().ContainInOrder(2, 3, 1);
    }

    private async Task<NotificationDto> Read()
    {
        var result = await _handler.Handle(
            new GetNotificationsQuery(Unread: null, Page: 1, PageSize: 20), CancellationToken.None);
        return result.Value!.Items.Single();
    }

    /// <summary>In-memory stand-in that joins the actor on every read, like the EF repository.</summary>
    private sealed class FakeJoinOnReadRepository : INotificationRepository
    {
        public Dictionary<string, ApplicationUser> Users { get; } = new();
        private readonly List<Notification> _rows = [];
        private int _nextId = 1;

        public void Add(string recipientId, string actorId, DateTime createdAt) =>
            _rows.Add(new Notification
            {
                Id = _nextId++,
                RecipientId = recipientId,
                ActorId = actorId,
                Type = NotificationType.JourneyLiked,
                EntityType = "Journey",
                EntityId = "1",
                Read = false,
                CreatedAt = createdAt,
            });

        public Task<(IReadOnlyList<NotificationDto> Items, int TotalCount)> GetPagedAsync(
            string recipientId, bool? unread, int page, int pageSize, CancellationToken cancellationToken)
        {
            var rows = _rows
                .Where(r => r.RecipientId == recipientId)
                .Where(r => unread is null || r.Read != unread.Value)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r =>
                {
                    var actor = Users[r.ActorId]; // resolved at read time, not stored
                    return new NotificationDto
                    {
                        Id = r.Id,
                        Type = r.Type,
                        EntityType = r.EntityType,
                        EntityId = r.EntityId,
                        TargetUrl = string.Empty,
                        Read = r.Read,
                        CreatedAt = r.CreatedAt,
                        ActorId = r.ActorId,
                        ActorUsername = actor.Username,
                        ActorDisplayName = actor.DisplayName,
                        ActorAvatarUrl = actor.AvatarUrl,
                    };
                })
                .ToList();

            return Task.FromResult<(IReadOnlyList<NotificationDto>, int)>((rows, rows.Count));
        }

        public Task CreateAsync(Notification notification, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<bool> MarkReadAsync(int id, string recipientId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<int> MarkAllReadAsync(string recipientId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
