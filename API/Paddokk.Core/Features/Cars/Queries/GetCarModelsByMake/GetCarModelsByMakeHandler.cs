using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarModelsByMake;

public sealed class GetCarModelsByMakeHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarModelsByMakeQuery, CarModelsResponse>
{
    public async Task<CarModelsResponse> Handle(GetCarModelsByMakeQuery request, CancellationToken cancellationToken)
    {
        var models = await carRepository.GetCarModelsByMakeAsync(request.MakeId, cancellationToken);
        return new CarModelsResponse { Models = [.. models.Select(CarMapping.ToModelDto)] };
    }
}
