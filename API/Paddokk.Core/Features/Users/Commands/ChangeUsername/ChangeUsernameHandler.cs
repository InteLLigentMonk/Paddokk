using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Commands.ChangeUsername;

public sealed class ChangeUsernameHandler(IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<ChangeUsernameCommand, Result<UserDto>>
{
    public static readonly TimeSpan RateLimitWindow = TimeSpan.FromDays(30);

    public async Task<Result<UserDto>> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("User not found"));

        var newUsername = request.Username.Trim().ToLowerInvariant();

        if (newUsername == user.Username)
            return Result<UserDto>.Success(UserMapping.ToDto(user));

        if (user.LastUsernameChangeAt is { } last)
        {
            var nextAllowed = last + RateLimitWindow;
            if (DateTime.UtcNow < nextAllowed)
            {
                var daysLeft = (int)Math.Ceiling((nextAllowed - DateTime.UtcNow).TotalDays);
                return Result<UserDto>.Failure(Error.Conflict(
                    $"Username can be changed again in {daysLeft} day(s)",
                    ErrorCodes.UsernameChangeTooSoon));
            }
        }

        if (UsernameGenerator.Reserved.Contains(newUsername))
            return Result<UserDto>.Failure(Error.Conflict(
                "Username is reserved", ErrorCodes.UsernameReserved));

        if (await userRepository.UsernameExistsAsync(newUsername, cancellationToken))
            return Result<UserDto>.Failure(Error.Conflict(
                "Username is already taken", ErrorCodes.UsernameTaken));

        user.Username = newUsername;
        user.LastUsernameChangeAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<UserDto>.Success(UserMapping.ToDto(user));
    }
}
