using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.SubscribeToUserCar;

public sealed class SubscribeToUserCarHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<SubscribeToUserCarCommand, Result>
{
    public Task<Result> Handle(SubscribeToUserCarCommand request, CancellationToken cancellationToken) =>
        Subscriptions.SubscribeAsync(
            new SubjectLookup<UserCar>(
                Label: "Car",
                LoadAsync: ct => carRepository.GetCarByIdAsync(request.CarId, ct),
                PrincipalIdOf: car => car.PrincipalId),
            new SubscriptionOps<UserCarSubscription>(
                FindAsync: ct => carRepository.GetCarSubscriptionAsync(actor.UserId, request.CarId, ct),
                CreateAsync: (sub, ct) => carRepository.CreateCarSubscriptionAsync(sub, ct),
                UpdateAsync: (sub, ct) => carRepository.UpdateCarSubscriptionAsync(sub, ct)),
            newRelation: () => new UserCarSubscription
            {
                UserId = actor.UserId,
                UserCarId = request.CarId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            actorUserId: actor.UserId,
            cancellationToken);
}
