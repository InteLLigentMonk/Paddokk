using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.DeleteUserCar;

public sealed class DeleteUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<DeleteUserCarCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCarCommand request, CancellationToken cancellationToken)
    {
        var userCar = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);
        if (userCar is null)
            return Result.Failure(Error.NotFound($"Car {request.CarId} not found"));

        if (userCar.Journeys.Any(j => j.Status == JourneyStatus.Active))
            return Result.Failure(Error.Conflict("Cannot delete a car with active journeys"));

        if (userCar.IsPrimary)
        {
            var allCars = await carRepository.GetUserCarsAsync(actor.UserId, cancellationToken);
            var nextPrimary = allCars.FirstOrDefault(c => c.Id != request.CarId);
            if (nextPrimary is not null)
            {
                await carRepository.UnsetPrimaryCar(actor.UserId, cancellationToken);
                nextPrimary.IsPrimary = true;
                nextPrimary.UpdatedAt = DateTime.UtcNow;
                await carRepository.UpdateUserCarAsync(nextPrimary, cancellationToken);
            }
        }

        await carRepository.DeleteUserCarAsync(actor.UserId, request.CarId, cancellationToken);
        return Result.Success();
    }
}
