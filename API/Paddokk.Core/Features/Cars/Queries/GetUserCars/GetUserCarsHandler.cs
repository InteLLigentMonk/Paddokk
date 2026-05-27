using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCars;

public sealed class GetUserCarsHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetUserCarsQuery, PagedResult<UserCarDto>>
{
    public async Task<PagedResult<UserCarDto>> Handle(GetUserCarsQuery request, CancellationToken cancellationToken)
    {
        var (cars, totalCount) = await carRepository.GetUserCarsPagedAsync(
            actor.UserId, request.Page, request.PageSize, cancellationToken);

        var items = cars.Select(c => CarMapping.ToUserCarDto(c, actor.UserId)).ToList();

        return PagedResult<UserCarDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
