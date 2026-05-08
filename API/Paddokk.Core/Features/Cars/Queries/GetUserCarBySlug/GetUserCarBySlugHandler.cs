using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarBySlug;

public sealed class GetUserCarBySlugHandler(
    ICarRepository carRepository,
    IUserRepository userRepository,
    IActorResolver actor)
    : IRequestHandler<GetUserCarBySlugQuery, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(GetUserCarBySlugQuery request, CancellationToken cancellationToken)
    {
        var owner = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (owner is null)
            return Result<UserCarDto>.Failure(Error.NotFound($"User '{request.Username}' not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var car = await carRepository.GetUserCarBySlugAsync(
            request.Username, request.Slug, currentUserId, cancellationToken);

        if (car is null)
            return Result<UserCarDto>.Failure(Error.NotFound($"Car '{request.Slug}' not found"));

        return Result<UserCarDto>.Success(CarMapping.ToUserCarDto(car, currentUserId));
    }
}
