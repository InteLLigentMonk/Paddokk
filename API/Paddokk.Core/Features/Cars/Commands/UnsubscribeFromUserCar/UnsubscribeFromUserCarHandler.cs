using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.UnsubscribeFromUserCar;

public sealed class UnsubscribeFromUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<UnsubscribeFromUserCarCommand, Result>
{
    public Task<Result> Handle(UnsubscribeFromUserCarCommand request, CancellationToken cancellationToken) =>
        Subscriptions.UnsubscribeAsync(
            new ToggleOps<UserCarSubscription>(
                FindAsync: ct => carRepository.GetCarSubscriptionAsync(actor.UserId, request.CarId, ct),
                UpdateAsync: (sub, ct) => carRepository.UpdateCarSubscriptionAsync(sub, ct)),
            cancellationToken);
}
