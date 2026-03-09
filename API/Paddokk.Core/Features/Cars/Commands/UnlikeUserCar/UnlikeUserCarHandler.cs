using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.UnlikeUserCar;

public sealed class UnlikeUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<UnlikeUserCarCommand, Result>
{
    public async Task<Result> Handle(UnlikeUserCarCommand request, CancellationToken cancellationToken)
    {
        var like = await carRepository.GetCarLikeAsync(actor.UserId, request.CarId, cancellationToken);

        if (like is null)
            return Result.Success(); // idempotent

        await carRepository.DeleteCarLikeAsync(actor.UserId, request.CarId, cancellationToken);
        return Result.Success();
    }
}
