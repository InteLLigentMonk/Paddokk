using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarGenerationsByModel;

public sealed class GetCarGenerationsByModelHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarGenerationsByModelQuery, CarGenerationsResponse>
{
    public async Task<CarGenerationsResponse> Handle(GetCarGenerationsByModelQuery request, CancellationToken cancellationToken)
    {
        var generations = await carRepository.GetCarGenerationsByModelAsync(request.ModelId, cancellationToken);
        return new CarGenerationsResponse { Generations = [.. generations.Select(CarMapping.ToGenerationDto)] };
    }
}
