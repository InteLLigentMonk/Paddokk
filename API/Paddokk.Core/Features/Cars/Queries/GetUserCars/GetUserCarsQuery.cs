using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCars;

public record GetUserCarsQuery : IQuery<UserCarsResponse>;
