using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByEmail;

public sealed class GetUserByEmailHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(Error.NotFound($"User with email '{request.Email}' not found"))
            : Result<UserDto>.Success(UserMapping.ToDto(user));
    }
}
