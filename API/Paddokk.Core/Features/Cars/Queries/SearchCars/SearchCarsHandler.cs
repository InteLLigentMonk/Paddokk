using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.SearchCars;

public sealed class SearchCarsHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<SearchCarsQuery, Result<PagedUserCarsResponse>>
{
    public async Task<Result<PagedUserCarsResponse>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
    {
        var (cars, total) = await carRepository.SearchCarsAsync(
            request.Terms,
            request.IsPublic,
            request.Sort,
            request.Page,
            request.PageSize,
            cancellationToken);

        var hasMore = (long)request.Page * request.PageSize < total;

        return Result<PagedUserCarsResponse>.Success(new PagedUserCarsResponse
        {
            Cars = [.. cars.Select(c => CarMapping.ToUserCarDto(c, actor.UserId))],
            TotalCount = total,
            HasMore = hasMore
        });
    }
}
