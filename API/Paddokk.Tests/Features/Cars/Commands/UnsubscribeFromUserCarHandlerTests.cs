using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.UnsubscribeFromUserCar;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

public class UnsubscribeFromUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UnsubscribeFromUserCarHandler _handler;

    public UnsubscribeFromUserCarHandlerTests()
    {
        _handler = new UnsubscribeFromUserCarHandler(_carRepo, _actor);
    }

    [Fact]
    public async Task Handle_NoExistingSubscription_IsIdempotent()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns((UserCarSubscription?)null);

        var result = await _handler.Handle(new UnsubscribeFromUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.DidNotReceive().UpdateCarSubscriptionAsync(Arg.Any<UserCarSubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingSubscription_SoftDeletes()
    {
        _actor.UserId.Returns("visitor-1");
        var existing = new UserCarSubscription { UserId = "visitor-1", UserCarId = 1, IsActive = true };
        _carRepo.GetCarSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new UnsubscribeFromUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeFalse();
        await _carRepo.Received(1).UpdateCarSubscriptionAsync(existing, Arg.Any<CancellationToken>());
    }
}
