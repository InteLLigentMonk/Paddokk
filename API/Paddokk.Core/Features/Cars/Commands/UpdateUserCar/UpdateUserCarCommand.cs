using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.UpdateUserCar;

public record UpdateUserCarCommand(
    int CarId,
    string? CustomBuildName,
    string? Nickname,
    string? Color,
    string? Region,
    CarDriveType? Drive,
    string? Engine,
    int? OdometerKm,
    string? OwnerNote,
    List<CarSpecCategoryDto>? SpecsByCategory,
    bool? IsPrimary
) : ICommand<Result<UserCarDto>>;
