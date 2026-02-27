using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarModelById;

public record GetCarModelByIdQuery(int ModelId) : IQuery<Result<CarModelDto>>;
