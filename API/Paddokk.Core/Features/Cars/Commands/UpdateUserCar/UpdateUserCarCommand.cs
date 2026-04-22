using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Commands.UpdateUserCar;

public record UpdateUserCarCommand(
    int CarId,
    string? CustomBuildName,
    string? Nickname,
    string? Color,
    string? Description,
    bool? IsPrimary
) : ICommand<Result<UserCarDto>>;
