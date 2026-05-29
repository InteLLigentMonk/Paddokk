using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Follows.Commands.UnfollowUser;

public sealed class UnfollowUserHandler(IUserFollowRepository followRepository, IActorResolver actor)
    : IRequestHandler<UnfollowUserCommand, Result>
{
    public Task<Result> Handle(UnfollowUserCommand request, CancellationToken cancellationToken) =>
        Subscriptions.UnsubscribeAsync(
            new ToggleOps<UserFollow>(
                FindAsync: ct => followRepository.GetFollowAsync(actor.UserId, request.FollowedUserId, ct),
                UpdateAsync: (follow, ct) => followRepository.UpdateFollowAsync(follow, ct)),
            cancellationToken);
}
