using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.SubscribeToUserCar;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

public class SubscribeToUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly SubscribeToUserCarHandler _handler;

    public SubscribeToUserCarHandlerTests()
    {
        _handler = new SubscribeToUserCarHandler(_carRepo, _actor);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(999, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new SubscribeToUserCarCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_OwnerSubscribesToOwnCar_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));

        var result = await _handler.Handle(new SubscribeToUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_ExistingActiveSubscription_IsNoOp()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new UserCarSubscription { UserId = "visitor-1", UserCarId = 1, IsActive = true });

        var result = await _handler.Handle(new SubscribeToUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.DidNotReceive().CreateCarSubscriptionAsync(Arg.Any<UserCarSubscription>(), Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().UpdateCarSubscriptionAsync(Arg.Any<UserCarSubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingInactiveSubscription_Reactivates()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        var existing = new UserCarSubscription { UserId = "visitor-1", UserCarId = 1, IsActive = false };
        _carRepo.GetCarSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new SubscribeToUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeTrue();
        await _carRepo.Received(1).UpdateCarSubscriptionAsync(existing, Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().CreateCarSubscriptionAsync(Arg.Any<UserCarSubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoExistingSubscription_CreatesNew()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((UserCarSubscription?)null);

        var result = await _handler.Handle(new SubscribeToUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).CreateCarSubscriptionAsync(
            Arg.Is<UserCarSubscription>(s => s.UserId == "visitor-1" && s.UserCarId == 1 && s.IsActive),
            Arg.Any<CancellationToken>());
    }

    private static UserCar BuildCar(string principalId) => new()
    {
        Id = 1,
        PrincipalId = principalId,
        Slug = "test-car",
        IsPublic = true,
        IsActive = true,
        IsCustomBuild = true,
        CustomBuildName = "Test Build",
        Journeys = [],
        Likes = [],
        Subscriptions = []
    };
}
