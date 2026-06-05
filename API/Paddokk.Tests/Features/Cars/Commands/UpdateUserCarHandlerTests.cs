using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Cars.Commands.UpdateUserCar;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Cars.Commands;

// UpdateUserCar must bump UserCar.UpdatedAt only when a spec field actually changes, so the
// Feed's SpecChanged event (#188) reflects mechanical evolution and not typo fixes, note
// edits, or no-op saves.
public class UpdateUserCarHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UpdateUserCarHandler _handler;

    private static readonly DateTime OriginalUpdatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public UpdateUserCarHandlerTests()
    {
        _actor.UserId.Returns("owner-1");
        _handler = new UpdateUserCarHandler(_carRepo, _actor);
    }

    private UserCar ArrangeCar(Action<UserCar>? configure = null)
    {
        var car = new UserCar
        {
            Id = 1,
            PrincipalId = "owner-1",
            Slug = "test-car",
            Engine = "B18C",
            Color = "Red",
            OdometerKm = 100_000,
            OwnerNote = "original note",
            Nickname = "Project",
            UpdatedAt = OriginalUpdatedAt
        };
        configure?.Invoke(car);
        _carRepo.GetUserCarByIdAsync("owner-1", 1, Arg.Any<CancellationToken>()).Returns(car);
        return car;
    }

    private static UpdateUserCarCommand Command(
        string? engine = null,
        string? color = null,
        int? odometerKm = null,
        string? ownerNote = null,
        string? nickname = null,
        List<CarSpecCategoryDto>? specs = null)
        => new(
            CarId: 1,
            CustomBuildName: null,
            Nickname: nickname,
            Color: color,
            Region: null,
            Drive: null,
            Engine: engine,
            OdometerKm: odometerKm,
            OwnerNote: ownerNote,
            SpecsByCategory: specs,
            IsPrimary: null);

    [Fact]
    public async Task Handle_SpecFieldChanged_BumpsUpdatedAt()
    {
        var car = ArrangeCar();

        await _handler.Handle(Command(engine: "K20A"), CancellationToken.None);

        car.UpdatedAt.Should().BeAfter(OriginalUpdatedAt);
    }

    [Fact]
    public async Task Handle_SpecsByCategoryChanged_BumpsUpdatedAt()
    {
        var car = ArrangeCar(c =>
            c.SpecsByCategory = [new CarSpecCategory { Category = "Engine", Items = ["Turbo"] }]);

        var specs = new List<CarSpecCategoryDto>
        {
            new() { Category = "Engine", Items = ["Turbo", "Intercooler"] }
        };
        await _handler.Handle(Command(specs: specs), CancellationToken.None);

        car.UpdatedAt.Should().BeAfter(OriginalUpdatedAt);
    }

    [Fact]
    public async Task Handle_NotesOnlyChanged_DoesNotBumpUpdatedAt()
    {
        var car = ArrangeCar();

        await _handler.Handle(Command(ownerNote: "a fresh note"), CancellationToken.None);

        car.UpdatedAt.Should().Be(OriginalUpdatedAt);
    }

    [Fact]
    public async Task Handle_NicknameOnlyChanged_DoesNotBumpUpdatedAt()
    {
        var car = ArrangeCar();

        await _handler.Handle(Command(nickname: "The Beast"), CancellationToken.None);

        car.UpdatedAt.Should().Be(OriginalUpdatedAt);
    }

    [Fact]
    public async Task Handle_NoOpSave_DoesNotBumpUpdatedAt()
    {
        var car = ArrangeCar();

        // Every provided value equals the current one — nothing actually changes.
        await _handler.Handle(Command(engine: "B18C", color: "Red", odometerKm: 100_000), CancellationToken.None);

        car.UpdatedAt.Should().Be(OriginalUpdatedAt);
    }
}
