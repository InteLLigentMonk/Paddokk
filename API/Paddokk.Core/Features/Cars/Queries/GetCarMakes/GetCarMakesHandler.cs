using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetCarMakes;

public sealed class GetCarMakesHandler(ICarRepository carRepository)
    : IRequestHandler<GetCarMakesQuery, CarMakesResponse>
{
    public async Task<CarMakesResponse> Handle(GetCarMakesQuery request, CancellationToken cancellationToken)
    {
        var makes = await carRepository.GetCarMakesAsync(cancellationToken);
        return new CarMakesResponse { Makes = [.. makes.Select(CarMapping.ToMakeDto)] };
    }
}
