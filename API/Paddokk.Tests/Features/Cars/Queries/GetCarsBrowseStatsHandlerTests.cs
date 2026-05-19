using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Queries.GetCarsBrowseStats;
using Paddokk.Core.Interfaces;

namespace Paddokk.Tests.Features.Cars.Queries;

public class GetCarsBrowseStatsHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly GetCarsBrowseStatsHandler _handler;

    public GetCarsBrowseStatsHandlerTests()
    {
        _handler = new GetCarsBrowseStatsHandler(_carRepo);
    }

    [Fact]
    public async Task Handle_EmptyTerms_ReturnsStatsForAllPublicCars()
    {
        var expected = new GetCarsBrowseStatsResponse { Cars = 42, Makes = 5, Owners = 30, Journeys = 120 };
        _carRepo.GetBrowseStatsAsync(
            Arg.Is<IReadOnlyList<string>>(t => t.Count == 0),
            true,
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(
            new GetCarsBrowseStatsQuery([], IsPublic: true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Cars.Should().Be(42);
        result.Value.Makes.Should().Be(5);
        result.Value.Owners.Should().Be(30);
        result.Value.Journeys.Should().Be(120);
    }

    [Fact]
    public async Task Handle_WithTerms_PassesTermsToRepository()
    {
        var expected = new GetCarsBrowseStatsResponse { Cars = 3, Makes = 1, Owners = 3, Journeys = 10 };
        _carRepo.GetBrowseStatsAsync(
            Arg.Is<IReadOnlyList<string>>(t => t.Count == 2 && t[0] == "bmw" && t[1] == "e30"),
            Arg.Any<bool?>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(
            new GetCarsBrowseStatsQuery(["bmw", "e30"]),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Cars.Should().Be(3);
    }

    [Fact]
    public async Task Handle_NoCarsMatch_ReturnsZeroStats()
    {
        _carRepo.GetBrowseStatsAsync(
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<bool?>(),
            Arg.Any<CancellationToken>())
            .Returns(new GetCarsBrowseStatsResponse());

        var result = await _handler.Handle(
            new GetCarsBrowseStatsQuery(["nonexistent"]),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Cars.Should().Be(0);
        result.Value.Makes.Should().Be(0);
        result.Value.Owners.Should().Be(0);
        result.Value.Journeys.Should().Be(0);
    }
}
