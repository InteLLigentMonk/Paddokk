using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarModelById;

public sealed class GetCarModelByIdHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarModelByIdQuery, Result<CarModelDto>>
{
    public async Task<Result<CarModelDto>> Handle(GetCarModelByIdQuery request, CancellationToken cancellationToken)
    {
        var model = await carRepository.GetCarModelByIdAsync(request.ModelId, cancellationToken);

        return model is null
            ? Result<CarModelDto>.Failure(Error.NotFound($"Car model {request.ModelId} not found"))
            : Result<CarModelDto>.Success(CarMapping.ToModelDto(model));
    }
}
