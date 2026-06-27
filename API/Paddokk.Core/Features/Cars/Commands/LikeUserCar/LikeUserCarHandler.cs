using MediatR;
using Paddokk.Core.Features.Cars.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.LikeUserCar;

public sealed class LikeUserCarHandler(
    ICarRepository carRepository,
    IActorResolver actor,
    IPublisher publisher)
    : IRequestHandler<LikeUserCarCommand, Result>
{
    public async Task<Result> Handle(LikeUserCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);

        if (car is null)
            return Result.Failure(Error.NotFound($"Car {request.CarId} not found"));

        if (car.PrincipalId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot like your own car", ErrorCodes.LikeOwnSubject));

        var existing = await carRepository.GetCarLikeAsync(actor.UserId, request.CarId, cancellationToken);

        if (existing is not null)
            return Result.Success(); // idempotent — no event, no duplicate notification

        await carRepository.CreateCarLikeAsync(new UserCarLike
        {
            UserId = actor.UserId,
            UserCarId = request.CarId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await publisher.Publish(
            new CarLiked(actor.UserId, request.CarId, car.PrincipalId),
            cancellationToken);

        return Result.Success();
    }
}
