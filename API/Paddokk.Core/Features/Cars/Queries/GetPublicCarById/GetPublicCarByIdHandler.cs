using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetPublicCarById;

public sealed class GetPublicCarByIdHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetPublicCarByIdQuery, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(GetPublicCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);

        return car is null
            ? Result<UserCarDto>.Failure(Error.NotFound($"Car {request.CarId} not found"))
            : Result<UserCarDto>.Success(CarMapping.ToUserCarDto(car, actor.UserId));
    }
}
