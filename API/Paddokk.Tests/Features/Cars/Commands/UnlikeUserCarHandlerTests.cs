using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.UnlikeUserCar;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

public class UnlikeUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UnlikeUserCarHandler _handler;

    public UnlikeUserCarHandlerTests()
    {
        _handler = new UnlikeUserCarHandler(_carRepo, _actor);
    }

    [Fact]
    public async Task Handle_OwnerUnlikesOwnCar_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));

        var result = await _handler.Handle(new UnlikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(999, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new UnlikeUserCarCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_NonOwnerUnlikesCar_DeletesLike()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new UserCarLike { UserId = "visitor-1", UserCarId = 1, CreatedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new UnlikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).DeleteCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonOwnerWithNoExistingLike_IsIdempotent()
    {
        _actor.UserId.Returns("visitor-1");
        _carRepo.GetCarByIdAsync(1, Arg.Any<CancellationToken>()).Returns(BuildCar(principalId: "owner-1"));
        _carRepo.GetCarLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((UserCarLike?)null);

        var result = await _handler.Handle(new UnlikeUserCarCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.DidNotReceive().DeleteCarLikeAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
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
