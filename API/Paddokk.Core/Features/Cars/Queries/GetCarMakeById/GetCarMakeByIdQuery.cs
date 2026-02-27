using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarMakeById;

public record GetCarMakeByIdQuery(int MakeId) : IQuery<Result<CarMakeDto>>;
