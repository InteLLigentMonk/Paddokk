using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.UpdateUserCar;

public sealed class UpdateUserCarHandler(
    ICarRepository carRepository,
    IActorResolver actor)
    : IRequestHandler<UpdateUserCarCommand, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(UpdateUserCarCommand request, CancellationToken cancellationToken)
    {
        var userCar = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);
        if (userCar is null)
            return Result<UserCarDto>.Failure(Error.NotFound($"Car {request.CarId} not found"));

        // Decide BEFORE mutating whether any spec field actually changes. UpdatedAt is the
        // Feed's SpecChanged signal (#188), so a notes-only edit or a no-op save must leave it
        // untouched — only the build's mechanical evolution should surface.
        var specChanged = SpecFieldsChanged(request, userCar);

        if (request.CustomBuildName is not null)
            userCar.CustomBuildName = string.IsNullOrEmpty(request.CustomBuildName) ? null : request.CustomBuildName;

        if (request.Nickname is not null)
            userCar.Nickname = string.IsNullOrEmpty(request.Nickname) ? null : request.Nickname;

        if (request.Color is not null)
            userCar.Color = string.IsNullOrEmpty(request.Color) ? null : request.Color;

        if (request.Region is not null)
            userCar.Region = string.IsNullOrEmpty(request.Region) ? null : request.Region;

        if (request.Drive is not null)
            userCar.Drive = request.Drive;

        if (request.Engine is not null)
            userCar.Engine = string.IsNullOrEmpty(request.Engine) ? null : request.Engine;

        if (request.OdometerKm.HasValue)
            userCar.OdometerKm = request.OdometerKm;

        if (request.OwnerNote is not null)
            userCar.OwnerNote = string.IsNullOrEmpty(request.OwnerNote) ? null : request.OwnerNote;

        if (request.SpecsByCategory is not null)
            userCar.SpecsByCategory = request.SpecsByCategory
                .Select(s => new CarSpecCategory { Category = s.Category, Items = s.Items })
                .ToList();

        if (request.IsPrimary.HasValue)
            userCar.IsPrimary = request.IsPrimary.Value;

        userCar.SearchText = CarSearchTextBuilder.Build(
            userCar.CarMake?.Name,
            userCar.CarModel?.Name,
            userCar.CarGeneration?.Name,
            userCar.CustomBuildName,
            userCar.Nickname,
            userCar.Year);

        if (specChanged)
            userCar.UpdatedAt = DateTime.UtcNow;

        if (request.IsPrimary == true)
            await carRepository.UnsetPrimaryCar(actor.UserId, cancellationToken);

        await carRepository.UpdateUserCarAsync(userCar, cancellationToken);

        var updated = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);
        return Result<UserCarDto>.Success(CarMapping.ToUserCarDto(updated!));
    }

    // Spec fields are the build's mechanical/physical attributes. Naming (Nickname,
    // CustomBuildName), the owner note, and the primary-image choice are deliberately excluded
    // — editing them is not a SpecChanged event.
    private static bool SpecFieldsChanged(UpdateUserCarCommand request, UserCar current) =>
        StringChanged(request.Color, current.Color)
        || StringChanged(request.Region, current.Region)
        || StringChanged(request.Engine, current.Engine)
        || (request.Drive is not null && request.Drive != current.Drive)
        || (request.OdometerKm.HasValue && request.OdometerKm != current.OdometerKm)
        || (request.SpecsByCategory is not null && SpecsChanged(request.SpecsByCategory, current.SpecsByCategory));

    // Mirrors the handler's own empty-string-to-null normalisation so "" over a null field is
    // not counted as a change.
    private static bool StringChanged(string? incoming, string? current) =>
        incoming is not null && (string.IsNullOrEmpty(incoming) ? null : incoming) != current;

    private static bool SpecsChanged(List<CarSpecCategoryDto> incoming, List<CarSpecCategory> current)
    {
        if (incoming.Count != current.Count)
            return true;

        for (var i = 0; i < incoming.Count; i++)
        {
            if (incoming[i].Category != current[i].Category
                || !incoming[i].Items.SequenceEqual(current[i].Items))
                return true;
        }

        return false;
    }
}
