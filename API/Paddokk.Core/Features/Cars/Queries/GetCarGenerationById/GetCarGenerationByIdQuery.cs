using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarGenerationById;

public record GetCarGenerationByIdQuery(int GenerationId) : IQuery<Result<CarGenerationDto>>;
