using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.CreateUserCar;

public record CreateUserCarCommand(
    SubscriptionTier SubscriptionTier,
    int CarMakeId,
    int CarModelId,
    int? CarGenerationId,
    int Year,
    string? Nickname,
    string? Color,
    string? Description,
    bool IsPrimary = false
) : ICommand<Result<UserCarDto>>;
