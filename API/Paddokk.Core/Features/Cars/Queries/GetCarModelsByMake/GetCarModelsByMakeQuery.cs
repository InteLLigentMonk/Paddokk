using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarModelsByMake;

public record GetCarModelsByMakeQuery(int MakeId) : IQuery<CarModelsResponse>;
