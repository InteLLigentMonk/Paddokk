using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.UnlikeUserCar;

public sealed class UnlikeUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<UnlikeUserCarCommand, Result>
{
    public async Task<Result> Handle(UnlikeUserCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);

        if (car is null)
            return Result.Failure(Error.NotFound($"Car {request.CarId} not found"));

        if (car.PrincipalId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot unlike your own car"));

        var like = await carRepository.GetCarLikeAsync(actor.UserId, request.CarId, cancellationToken);

        if (like is null)
            return Result.Success(); // idempotent

        await carRepository.DeleteCarLikeAsync(actor.UserId, request.CarId, cancellationToken);
        return Result.Success();
    }
}
