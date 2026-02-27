using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Queries.GetCarLimits;

public sealed class GetCarLimitsHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetCarLimitsQuery, CarLimitDto>
{
    public async Task<CarLimitDto> Handle(GetCarLimitsQuery request, CancellationToken cancellationToken)
    {
        var currentCount = await carRepository.GetUserCarCountAsync(actor.UserId, cancellationToken);

        var maxCars = request.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };

        return new CarLimitDto
        {
            CanAdd = currentCount < maxCars,
            CurrentCount = currentCount,
            MaxAllowed = maxCars == int.MaxValue ? "Unlimited" : maxCars.ToString(),
            SubscriptionTier = request.SubscriptionTier.ToString()
        };
    }
}
