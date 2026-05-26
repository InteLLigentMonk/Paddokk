using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.LikeUserCar;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

public class LikeUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly LikeUserCarHandler _handler;

    public LikeUserCarHandlerTests()
    {
        _handler = new LikeUserCarHandler(_carRepo, _actor);
    }

    [Fact]
    public async Task Handle_OwnerLikesOwnCar_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(999, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new LikeUserCarCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_NonOwnerLikesCar_CreatesLike()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((UserCarLike?)null);

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).CreateCarLikeAsync(
            Arg.Is<UserCarLike>(l => l.UserId == "visitor-1" && l.UserCarId == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonOwnerAlreadyLiked_IsIdempotent()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new UserCarLike { UserId = "visitor-1", UserCarId = 1, CreatedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new LikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.DidNotReceive().CreateCarLikeAsync(Arg.Any<UserCarLike>(), Arg.Any<CancellationToken>());
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
