using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.SubscribeToUserCar;

public sealed class SubscribeToUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<SubscribeToUserCarCommand, Result>
{
    public async Task<Result> Handle(SubscribeToUserCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);

        if (car is null)
            return Result.Failure(Error.NotFound($"Car {request.CarId} not found"));

        if (car.UserId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot subscribe to your own car"));

        var existing = await carRepository.GetCarSubscriptionAsync(actor.UserId, request.CarId, cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await carRepository.UpdateCarSubscriptionAsync(existing, cancellationToken);
            }

            return Result.Success();
        }

        await carRepository.CreateCarSubscriptionAsync(new UserCarSubscription
        {
            UserId = actor.UserId,
            UserCarId = request.CarId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }, cancellationToken);

        return Result.Success();
    }
}
