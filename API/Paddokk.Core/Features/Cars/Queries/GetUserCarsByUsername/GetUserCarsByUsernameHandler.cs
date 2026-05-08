using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarsByUsername;

public sealed class GetUserCarsByUsernameHandler(
    ICarRepository carRepository,
    IUserRepository userRepository,
    IActorResolver actor)
    : IRequestHandler<GetUserCarsByUsernameQuery, Result<IEnumerable<UserCarDto>>>
{
    public async Task<Result<IEnumerable<UserCarDto>>> Handle(
        GetUserCarsByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        var owner = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (owner is null)
            return Result<IEnumerable<UserCarDto>>.Failure(Error.NotFound($"User '{request.Username}' not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var cars = await carRepository.GetUserCarsByUsernameAsync(request.Username, currentUserId, cancellationToken);

        return Result<IEnumerable<UserCarDto>>.Success(cars.Select(c => CarMapping.ToUserCarDto(c, currentUserId)));
    }
}
