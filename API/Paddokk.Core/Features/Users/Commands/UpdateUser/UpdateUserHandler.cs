using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler(IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("User not found"));

        if (!string.IsNullOrEmpty(request.DisplayName))
            user.DisplayName = request.DisplayName;

        if (request.Bio is not null)
            user.Bio = request.Bio;

        if (request.AvatarUrl is not null)
            user.AvatarUrl = request.AvatarUrl;

        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<UserDto>.Success(UserMapping.ToDto(user));
    }
}
