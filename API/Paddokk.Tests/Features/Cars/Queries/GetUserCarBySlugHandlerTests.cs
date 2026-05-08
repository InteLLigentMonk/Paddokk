using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Queries.GetUserCarBySlug;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Cars.Queries;

public class GetUserCarBySlugHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetUserCarBySlugHandler _handler;

    public GetUserCarBySlugHandlerTests()
    {
        _handler = new GetUserCarBySlugHandler(_carRepo, _userRepo, _actor);
    }

    [Fact]
    public async Task Handle_PublicCar_NonOwner_ReturnsCar()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.UserId.Returns("visitor-1");
        _actor.IsAuthenticated.Returns(true);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo.GetUserCarBySlugAsync("owner", "my-car", "visitor-1", Arg.Any<CancellationToken>())
            .Returns(BuildCar(owner, "my-car", isPublic: true));

        var result = await _handler.Handle(new GetUserCarBySlugQuery("owner", "my-car"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Slug.Should().Be("my-car");
    }

    [Fact]
    public async Task Handle_PrivateCar_NonOwner_ReturnsNotFound()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.UserId.Returns("visitor-1");
        _actor.IsAuthenticated.Returns(true);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo.GetUserCarBySlugAsync("owner", "secret-car", "visitor-1", Arg.Any<CancellationToken>())
            .Returns((UserCar?)null);

        var result = await _handler.Handle(new GetUserCarBySlugQuery("owner", "secret-car"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(Paddokk.Core.Models.ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_PrivateCar_Owner_ReturnsCar()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.Username = "owner";
        _actor.UserId.Returns("owner-1");
        _actor.IsAuthenticated.Returns(true);
        _userRepo.GetByUsernameAsync("owner", Arg.Any<CancellationToken>()).Returns(owner);
        _carRepo.GetUserCarBySlugAsync("owner", "secret-car", "owner-1", Arg.Any<CancellationToken>())
            .Returns(BuildCar(owner, "secret-car", isPublic: false));

        var result = await _handler.Handle(new GetUserCarBySlugQuery("owner", "secret-car"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsPublic.Should().BeFalse();
        result.Value.IsOwner.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UnknownUsername_ReturnsNotFound()
    {
        _actor.UserId.Returns((string?)null);
        _userRepo.GetByUsernameAsync("ghost", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new GetUserCarBySlugQuery("ghost", "any"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(Paddokk.Core.Models.ErrorType.NotFound);
    }

    private static UserCar BuildCar(ApplicationUser owner, string slug, bool isPublic) => new()
    {
        Id = 1,
        PrincipalId = owner.Id,
        User = owner,
        Slug = slug,
        IsPublic = isPublic,
        IsActive = true,
        IsCustomBuild = true,
        CustomBuildName = "Test Build",
        Journeys = [],
        Likes = [],
        Subscriptions = []
    };
}
