using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByUsername;

public sealed class GetUserByUsernameHandler(IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<GetUserByUsernameQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var actorUserId = actor.IsAuthenticated ? actor.UserId : null;
        var profile = await userRepository.GetProfileByUsernameAsync(request.Username, actorUserId, cancellationToken);

        return profile is null
            ? Result<UserDto>.Failure(Error.NotFound($"User '{request.Username}' not found"))
            : Result<UserDto>.Success(UserMapping.ToDto(profile));
    }
}
