using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarGenerationsByModel;

public record GetCarGenerationsByModelQuery(int ModelId) : IQuery<CarGenerationsResponse>;
