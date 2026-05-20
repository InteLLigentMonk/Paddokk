using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Commands.CreateUserCar;

public record CreateUserCarCommand(
    bool IsCustomBuild,
    string? CustomBuildName,
    int? CarMakeId,
    int? CarModelId,
    int? CarGenerationId,
    int? Year,
    string? Nickname,
    string? Color,
    bool IsPrimary = false
) : ICommand<Result<UserCarDto>>;
