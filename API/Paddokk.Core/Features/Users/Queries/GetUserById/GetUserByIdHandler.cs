using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(Error.NotFound($"User '{request.UserId}' not found"))
            : Result<UserDto>.Success(UserMapping.ToDto(user));
    }
}
