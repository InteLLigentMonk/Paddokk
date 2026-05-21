using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Queries.SearchCars;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Cars.Queries;

public class SearchCarsHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly SearchCarsHandler _handler;

    public SearchCarsHandlerTests()
    {
        _handler = new SearchCarsHandler(_carRepo, _actor);
        _actor.UserId.Returns("user-1");
    }

    [Fact]
    public async Task Handle_EmptyTerms_ReturnsAllPublicCars()
    {
        var cars = new List<UserCar> { BuildCar(1, isPublic: true) };
        SetupRepo(cars, 1);

        var result = await _handler.Handle(
            new SearchCarsQuery([], IsPublic: true, Sort: CarSearchSort.Newest, Page: 1, PageSize: 24),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Cars.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.HasMore.Should().BeFalse();

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Is<IReadOnlyList<string>>(t => t.Count == 0),
            true,
            CarSearchSort.Newest,
            1,
            24,
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SingleTerm_PassesTermsToRepository()
    {
        var cars = new List<UserCar> { BuildCar(1) };
        SetupRepo(cars, 1);

        await _handler.Handle(
            new SearchCarsQuery(["bmw"], IsPublic: true),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Is<IReadOnlyList<string>>(t => t.Count == 1 && t[0] == "bmw"),
            true,
            Arg.Any<CarSearchSort>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MultipleTerms_PassesAllTermsToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new SearchCarsQuery(["bmw", "e30"], IsPublic: true),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Is<IReadOnlyList<string>>(t => t.Count == 2 && t[0] == "bmw" && t[1] == "e30"),
            true,
            Arg.Any<CarSearchSort>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MostLikedSort_PassesSortToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new SearchCarsQuery([], Sort: CarSearchSort.MostLiked),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<bool?>(),
            CarSearchSort.MostLiked,
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MostJourneysSort_PassesSortToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new SearchCarsQuery([], Sort: CarSearchSort.MostJourneys),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<bool?>(),
            CarSearchSort.MostJourneys,
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RelevanceSort_PassesSortToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new SearchCarsQuery(["volvo"], Sort: CarSearchSort.Relevance),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<bool?>(),
            CarSearchSort.Relevance,
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_HasMore_TrueWhenMorePagesExist()
    {
        var cars = Enumerable.Range(1, 24).Select(i => BuildCar(i)).ToList();
        SetupRepo(cars, 50);

        var result = await _handler.Handle(
            new SearchCarsQuery([], Page: 1, PageSize: 24),
            CancellationToken.None);

        result.Value!.HasMore.Should().BeTrue();
        result.Value.TotalCount.Should().Be(50);
    }

    [Fact]
    public async Task Handle_HasMore_FalseOnLastPage()
    {
        var cars = Enumerable.Range(1, 2).Select(i => BuildCar(i)).ToList();
        SetupRepo(cars, 26);

        var result = await _handler.Handle(
            new SearchCarsQuery([], Page: 2, PageSize: 24),
            CancellationToken.None);

        result.Value!.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_IsPublicFilter_PassedToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new SearchCarsQuery([], IsPublic: true),
            CancellationToken.None);

        await _carRepo.Received(1).SearchCarsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            true,
            Arg.Any<CarSearchSort>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsOwnerAvatarUrlFromCar()
    {
        var owner = JourneyTestHelpers.BuildUser("owner-1");
        owner.AvatarUrl = "https://example.com/avatar.jpg";
        var car = BuildCar(1);
        car.User = owner;
        SetupRepo([car], 1);

        var result = await _handler.Handle(
            new SearchCarsQuery([]),
            CancellationToken.None);

        result.Value!.Cars[0].OwnerAvatarUrl.Should().Be("https://example.com/avatar.jpg");
    }

    private void SetupRepo(List<UserCar> cars, int total)
    {
        _carRepo.SearchCarsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<bool?>(),
            Arg.Any<CarSearchSort>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns((cars, total));
    }

    private static UserCar BuildCar(int id, bool isPublic = true) => new()
    {
        Id = id,
        PrincipalId = "owner-1",
        Slug = $"car-{id}",
        IsPublic = isPublic,
        IsActive = true,
        IsCustomBuild = false,
        User = JourneyTestHelpers.BuildUser("owner-1"),
        Journeys = [],
        Likes = [],
        Subscriptions = []
    };
}
