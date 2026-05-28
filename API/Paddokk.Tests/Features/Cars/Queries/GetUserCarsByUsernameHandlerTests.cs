using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Queries.GetUserCarsByUsername;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Cars.Queries;

public class GetUserCarsByUsernameHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetUserCarsByUsernameHandler _handler;

    public GetUserCarsByUsernameHandlerTests()
    {
        _handler = new GetUserCarsByUsernameHandler(_carRepo, _userRepo, _actor);
    }

    [Fact]
    public async Task Handle_WithLimit_PassesEffectiveLimitToRepository()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.IsAuthenticated.Returns(false);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo
            .GetUserCarsByUsernameAsync("owner", null, 5, Arg.Any<CancellationToken>())
            .Returns(new List<UserCar>());

        var result = await _handler.Handle(new GetUserCarsByUsernameQuery("owner", Limit: 5), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).GetUserCarsByUsernameAsync("owner", null, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithLimitAboveMax_CapsAtMaxLimit()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.IsAuthenticated.Returns(false);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo
            .GetUserCarsByUsernameAsync("owner", null, 50, Arg.Any<CancellationToken>())
            .Returns(new List<UserCar>());

        var result = await _handler.Handle(new GetUserCarsByUsernameQuery("owner", Limit: 9999), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).GetUserCarsByUsernameAsync("owner", null, 50, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutLimit_PassesNullLimit()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.IsAuthenticated.Returns(false);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo
            .GetUserCarsByUsernameAsync("owner", null, (int?)null, Arg.Any<CancellationToken>())
            .Returns(new List<UserCar>());

        var result = await _handler.Handle(new GetUserCarsByUsernameQuery("owner"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _carRepo.Received(1).GetUserCarsByUsernameAsync("owner", null, (int?)null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownUsername_ReturnsNotFound()
    {
        _userRepo.GetByUsernameAsync("ghost", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new GetUserCarsByUsernameQuery("ghost", Limit: 5), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(Paddokk.Core.Models.ErrorType.NotFound);
    }
}
