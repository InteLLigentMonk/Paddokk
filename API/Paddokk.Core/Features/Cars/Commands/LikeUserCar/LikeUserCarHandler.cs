using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.LikeUserCar;

public sealed class LikeUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<LikeUserCarCommand, Result>
{
    public async Task<Result> Handle(LikeUserCarCommand request, CancellationToken cancellationToken)
    {
        var existing = await carRepository.GetCarLikeAsync(actor.UserId, request.CarId, cancellationToken);

        if (existing is not null)
            return Result.Success(); // idempotent

        await carRepository.CreateCarLikeAsync(new UserCarLike
        {
            UserId = actor.UserId,
            UserCarId = request.CarId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        return Result.Success();
    }
}
