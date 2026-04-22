using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.SearchCars;

public sealed class SearchCarsHandler(ICarRepository carRepository)
    : IRequestHandler<SearchCarsQuery, Result<UserCarsResponse>>
{
    public async Task<Result<UserCarsResponse>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Result<UserCarsResponse>.Failure(Error.Validation("Search query cannot be empty"));

        var cars = await carRepository.SearchCarsAsync(
            request.Query,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Result<UserCarsResponse>.Success(new UserCarsResponse
        {
            Cars = [.. cars.Select(c => CarMapping.ToUserCarDto(c))]
        });
    }
}
