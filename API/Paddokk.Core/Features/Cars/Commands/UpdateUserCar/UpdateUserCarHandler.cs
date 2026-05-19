using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Commands.UpdateUserCar;

public sealed class UpdateUserCarHandler(
    ICarRepository carRepository,
    IActorResolver actor,
    IHtmlSanitizationService htmlSanitizer)
    : IRequestHandler<UpdateUserCarCommand, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(UpdateUserCarCommand request, CancellationToken cancellationToken)
    {
        var userCar = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);
        if (userCar is null)
            return Result<UserCarDto>.Failure(Error.NotFound($"Car {request.CarId} not found"));

        if (request.CustomBuildName is not null)
            userCar.CustomBuildName = string.IsNullOrEmpty(request.CustomBuildName) ? null : request.CustomBuildName;

        if (request.Nickname is not null)
            userCar.Nickname = string.IsNullOrEmpty(request.Nickname) ? null : request.Nickname;

        if (request.Color is not null)
            userCar.Color = string.IsNullOrEmpty(request.Color) ? null : request.Color;

        if (request.Description is not null)
            userCar.Description = string.IsNullOrEmpty(request.Description) ? null : htmlSanitizer.Sanitize(request.Description);

        if (request.IsPrimary.HasValue)
            userCar.IsPrimary = request.IsPrimary.Value;

        userCar.SearchText = CarSearchTextBuilder.Build(
            userCar.CarMake?.Name,
            userCar.CarModel?.Name,
            userCar.CarGeneration?.Name,
            userCar.CustomBuildName,
            userCar.Nickname,
            userCar.Year);

        userCar.UpdatedAt = DateTime.UtcNow;

        if (request.IsPrimary == true)
            await carRepository.UnsetPrimaryCar(actor.UserId, cancellationToken);

        await carRepository.UpdateUserCarAsync(userCar, cancellationToken);

        var updated = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);
        return Result<UserCarDto>.Success(CarMapping.ToUserCarDto(updated!));
    }

}
