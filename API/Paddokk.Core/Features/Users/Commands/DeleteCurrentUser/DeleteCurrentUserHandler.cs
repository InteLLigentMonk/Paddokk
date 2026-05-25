using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Users.Commands.DeleteCurrentUser;

public sealed class DeleteCurrentUserHandler(IUserRepository userRepository, IActorResolver actor)
    : IRequestHandler<DeleteCurrentUserCommand, Result>
{
    public static readonly TimeSpan ReservationWindow = TimeSpan.FromDays(180);

    public async Task<Result> Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound("User not found"));

        var originalUsername = user.Username;
        var now = DateTime.UtcNow;
        var shortId = Guid.NewGuid().ToString("N")[..8];

        await userRepository.ReserveUsernameAsync(new ReservedUsername
        {
            Slug = originalUsername,
            OriginalUserId = user.Id,
            ReservedAt = now,
            ReleaseAfter = now + ReservationWindow
        }, cancellationToken);

        user.Username = $"deleted-{shortId}";
        user.DisplayName = "Deleted User";
        user.Email = null;
        user.Bio = null;
        user.AvatarUrl = null;
        user.FirstName = string.Empty;
        user.LastName = null;
        user.IsDeleted = true;
        user.DeletedAt = now;
        user.UpdatedAt = now;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
