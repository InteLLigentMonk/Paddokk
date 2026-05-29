using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Follows.Commands.UnfollowUser;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Follows.Commands;

public class UnfollowUserHandlerTests
{
    private readonly IUserFollowRepository _followRepo = Substitute.For<IUserFollowRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UnfollowUserHandler _handler;

    public UnfollowUserHandlerTests()
    {
        _handler = new UnfollowUserHandler(_followRepo, _actor);
    }

    [Fact]
    public async Task Handle_NoExistingFollow_IsIdempotent()
    {
        _actor.UserId.Returns("follower-1");
        _followRepo.GetFollowAsync("follower-1", "target-1", Arg.Any<CancellationToken>()).Returns((UserFollow?)null);

        var result = await _handler.Handle(new UnfollowUserCommand("target-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _followRepo.DidNotReceive().UpdateFollowAsync(Arg.Any<UserFollow>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingFollow_SoftDeletes()
    {
        _actor.UserId.Returns("follower-1");
        var existing = new UserFollow { FollowerId = "follower-1", FollowedId = "target-1", IsActive = true };
        _followRepo.GetFollowAsync("follower-1", "target-1", Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new UnfollowUserCommand("target-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeFalse();
        await _followRepo.Received(1).UpdateFollowAsync(existing, Arg.Any<CancellationToken>());
    }
}
