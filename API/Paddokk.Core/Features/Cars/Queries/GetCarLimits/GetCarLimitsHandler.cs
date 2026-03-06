using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Queries.GetCarLimits;

public sealed class GetCarLimitsHandler(ICarRepository carRepository, IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<GetCarLimitsQuery, CarLimitDto>
{
    public async Task<CarLimitDto> Handle(GetCarLimitsQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found");

        var currentCount = await carRepository.GetUserCarCountAsync(actor.UserId, cancellationToken);

        var maxCars = user.SubscriptionTier switch
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
            SubscriptionTier = user.SubscriptionTier.ToString()
        };
    }
}
