using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarLimits;

public record GetCarLimitsQuery : IQuery<CarLimitDto>;
