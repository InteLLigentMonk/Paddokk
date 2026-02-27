using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCars;

public sealed class GetUserCarsHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetUserCarsQuery, UserCarsResponse>
{
    public async Task<UserCarsResponse> Handle(GetUserCarsQuery request, CancellationToken cancellationToken)
    {
        var cars = await carRepository.GetUserCarsAsync(actor.UserId, cancellationToken);
        return new UserCarsResponse { Cars = [.. cars.Select(CarMapping.ToUserCarDto)] };
    }
}
