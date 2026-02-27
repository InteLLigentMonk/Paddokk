using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarById;

public sealed class GetUserCarByIdHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetUserCarByIdQuery, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(GetUserCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetUserCarByIdAsync(actor.UserId, request.CarId, cancellationToken);

        return car is null
            ? Result<UserCarDto>.Failure(Error.NotFound($"Car {request.CarId} not found"))
            : Result<UserCarDto>.Success(CarMapping.ToUserCarDto(car));
    }
}
