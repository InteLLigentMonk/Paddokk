using FluentAssertions;
using MediatR;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.LikeUserCar;
using Paddokk.Core.Features.Cars.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

public class LikeUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly LikeUserCarHandler _handler;

    public LikeUserCarHandlerTests()
    {
        _handler = new LikeUserCarHandler(_carRepo, _actor, _publisher);
    }

    [Fact]
    public async Task Handle_OwnerLikesOwnCar_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be(ErrorCodes.LikeOwnSubject);
        await _publisher.DidNotReceive().Publish(Arg.Any<CarLiked>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(999, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new LikeUserCarCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        await _publisher.DidNotReceive().Publish(Arg.Any<CarLiked>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonOwnerLikesCar_CreatesLikeAndPublishesEvent()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((UserCarLike?)null);

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).CreateCarLikeAsync(
            Arg.Is<UserCarLike>(l => l.UserId == "visitor-1" && l.UserCarId == 1),
            Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(
            Arg.Is<CarLiked>(e => e.ActorId == "visitor-1" && e.UserCarId == 1 && e.CarOwnerId == "owner-1"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonOwnerAlreadyLiked_IsIdempotentAndDoesNotPublish()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new UserCarLike { UserId = "visitor-1", UserCarId = 1, CreatedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.DidNotReceive().CreateCarLikeAsync(Arg.Any<UserCarLike>(), Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().Publish(Arg.Any<CarLiked>(), Arg.Any<CancellationToken>());
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
