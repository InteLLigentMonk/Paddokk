using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarMakeById;

public sealed class GetCarMakeByIdHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarMakeByIdQuery, Result<CarMakeDto>>
{
    public async Task<Result<CarMakeDto>> Handle(GetCarMakeByIdQuery request, CancellationToken cancellationToken)
    {
        var make = await carRepository.GetCarMakeByIdAsync(request.MakeId, cancellationToken);

        return make is null
            ? Result<CarMakeDto>.Failure(Error.NotFound($"Car make {request.MakeId} not found"))
            : Result<CarMakeDto>.Success(CarMapping.ToMakeDto(make));
    }
}
