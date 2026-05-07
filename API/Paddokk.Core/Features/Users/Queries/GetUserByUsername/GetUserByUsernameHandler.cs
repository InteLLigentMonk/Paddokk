using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByUsername;

public sealed class GetUserByUsernameHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByUsernameQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(Error.NotFound($"User '{request.Username}' not found"))
            : Result<UserDto>.Success(UserMapping.ToDto(user));
    }
}
