using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.SearchCars;

public record SearchCarsQuery(
    string Query,
    int Page = 1,
    int PageSize = 20
) : IQuery<Result<UserCarsResponse>>;
