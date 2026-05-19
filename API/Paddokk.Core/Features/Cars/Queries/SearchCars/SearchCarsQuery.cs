using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.SearchCars;

public record SearchCarsQuery(
    IReadOnlyList<string> Terms,
    bool? IsPublic = null,
    CarSearchSort Sort = CarSearchSort.Newest,
    int Page = 1,
    int PageSize = 24
) : IQuery<Result<PagedUserCarsResponse>>;
