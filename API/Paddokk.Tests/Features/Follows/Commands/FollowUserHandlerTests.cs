using FluentAssertions;
using MediatR;
using NSubstitute;
using Paddokk.Core.Features.Follows.Commands.FollowUser;
using Paddokk.Core.Features.Follows.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Follows.Commands;

public class FollowUserHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IUserFollowRepository _followRepo = Substitute.For<IUserFollowRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly FollowUserHandler _handler;

    public FollowUserHandlerTests()
    {
        _handler = new FollowUserHandler(_userRepo, _followRepo, _actor, _publisher);
    }

    private static ApplicationUser User(string id) => new() { Id = id };

    [Fact]
    public async Task Handle_FollowedUserNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("follower-1");
        _userRepo.GetByIdAsync("missing", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new FollowUserCommand("missing"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_FollowingYourself_ReturnsConflict()
    {
        _actor.UserId.Returns("me-1");
        _userRepo.GetByIdAsync("me-1", Arg.Any<CancellationToken>()).Returns(User("me-1"));

        var result = await _handler.Handle(new FollowUserCommand("me-1"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_ExistingActiveFollow_IsNoOp()
    {
        _actor.UserId.Returns("follower-1");
        _userRepo.GetByIdAsync("target-1", Arg.Any<CancellationToken>()).Returns(User("target-1"));
        _followRepo.GetFollowAsync("follower-1", "target-1", Arg.Any<CancellationToken>())
            .Returns(new UserFollow { FollowerId = "follower-1", FollowedId = "target-1", IsActive = true });

        var result = await _handler.Handle(new FollowUserCommand("target-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _followRepo.DidNotReceive().CreateFollowAsync(Arg.Any<UserFollow>(), Arg.Any<CancellationToken>());
        await _followRepo.DidNotReceive().UpdateFollowAsync(Arg.Any<UserFollow>(), Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().Publish(Arg.Any<UserFollowed>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingInactiveFollow_ReactivatesAndEmitsEvent()
    {
        _actor.UserId.Returns("follower-1");
        _userRepo.GetByIdAsync("target-1", Arg.Any<CancellationToken>()).Returns(User("target-1"));
        var existing = new UserFollow { FollowerId = "follower-1", FollowedId = "target-1", IsActive = false };
        _followRepo.GetFollowAsync("follower-1", "target-1", Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new FollowUserCommand("target-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeTrue();
        await _followRepo.Received(1).UpdateFollowAsync(existing, Arg.Any<CancellationToken>());
        await _followRepo.DidNotReceive().CreateFollowAsync(Arg.Any<UserFollow>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(
            Arg.Is<UserFollowed>(e => e.FollowerId == "follower-1" && e.FollowedId == "target-1"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoExistingFollow_CreatesAndEmitsEvent()
    {
        _actor.UserId.Returns("follower-1");
        _userRepo.GetByIdAsync("target-1", Arg.Any<CancellationToken>()).Returns(User("target-1"));
        _followRepo.GetFollowAsync("follower-1", "target-1", Arg.Any<CancellationToken>()).Returns((UserFollow?)null);

        var result = await _handler.Handle(new FollowUserCommand("target-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _followRepo.Received(1).CreateFollowAsync(
            Arg.Is<UserFollow>(f => f.FollowerId == "follower-1" && f.FollowedId == "target-1" && f.IsActive),
            Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(
            Arg.Is<UserFollowed>(e => e.FollowerId == "follower-1" && e.FollowedId == "target-1"),
            Arg.Any<CancellationToken>());
    }
}
