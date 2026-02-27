using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarById;

public record GetUserCarByIdQuery(int CarId) : IQuery<Result<UserCarDto>>;
