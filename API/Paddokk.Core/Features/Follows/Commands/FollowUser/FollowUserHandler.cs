using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Features.Follows.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Follows.Commands.FollowUser;

public sealed class FollowUserHandler(
    IUserRepository userRepository,
    IUserFollowRepository followRepository,
    IActorResolver actor,
    IPublisher publisher)
    : IRequestHandler<FollowUserCommand, Result>
{
    public Task<Result> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        // Both Create and Update run only when the relationship transitions to active
        // (no-op when already active), so emitting from each is exactly the
        // "on create + reactivate, never on no-op/unfollow" contract.
        var followed = request.FollowedUserId;

        return Subscriptions.SubscribeAsync(
            new SubjectLookup<ApplicationUser>(
                Label: "User",
                LoadAsync: ct => userRepository.GetByIdAsync(followed, ct),
                PrincipalIdOf: user => user.Id),
            new SubscriptionOps<UserFollow>(
                FindAsync: ct => followRepository.GetFollowAsync(actor.UserId, followed, ct),
                CreateAsync: async (follow, ct) =>
                {
                    await followRepository.CreateFollowAsync(follow, ct);
                    await publisher.Publish(new UserFollowed(actor.UserId, followed), ct);
                },
                UpdateAsync: async (follow, ct) =>
                {
                    await followRepository.UpdateFollowAsync(follow, ct);
                    await publisher.Publish(new UserFollowed(actor.UserId, followed), ct);
                }),
            newRelation: () => new UserFollow
            {
                FollowerId = actor.UserId,
                FollowedId = followed,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            actorUserId: actor.UserId,
            cancellationToken);
    }
}
