using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByEmail;

public sealed class GetUserByEmailHandler(IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var actorUserId = actor.IsAuthenticated ? actor.UserId : null;
        var profile = await userRepository.GetProfileByEmailAsync(request.Email, actorUserId, cancellationToken);

        return profile is null
            ? Result<UserDto>.Failure(Error.NotFound($"User with email '{request.Email}' not found"))
            : Result<UserDto>.Success(UserMapping.ToDto(profile));
    }
}
