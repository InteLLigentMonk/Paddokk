using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarGenerationById;

public sealed class GetCarGenerationByIdHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarGenerationByIdQuery, Result<CarGenerationDto>>
{
    public async Task<Result<CarGenerationDto>> Handle(GetCarGenerationByIdQuery request, CancellationToken cancellationToken)
    {
        var generation = await carRepository.GetCarGenerationByIdAsync(request.GenerationId, cancellationToken);

        return generation is null
            ? Result<CarGenerationDto>.Failure(Error.NotFound($"Car generation {request.GenerationId} not found"))
            : Result<CarGenerationDto>.Success(CarMapping.ToGenerationDto(generation));
    }
}
