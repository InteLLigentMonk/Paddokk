using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCars;

public record GetUserCarsQuery(int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<PagedResult<UserCarDto>>;
