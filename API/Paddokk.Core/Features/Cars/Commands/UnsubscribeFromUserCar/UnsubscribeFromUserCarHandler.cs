using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.UnsubscribeFromUserCar;

public sealed class UnsubscribeFromUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<UnsubscribeFromUserCarCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeFromUserCarCommand request, CancellationToken cancellationToken)
    {
        var subscription = await carRepository.GetCarSubscriptionAsync(actor.UserId, request.CarId, cancellationToken);

        if (subscription is null)
            return Result.Success(); // idempotent

        subscription.IsActive = false;
        await carRepository.UpdateCarSubscriptionAsync(subscription, cancellationToken);
        return Result.Success();
    }
}
