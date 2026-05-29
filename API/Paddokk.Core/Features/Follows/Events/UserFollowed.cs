using MediatR;

namespace Paddokk.Core.Features.Follows.Events;

/// <summary>
/// Raised when a follow relationship becomes active (created or reactivated).
/// Not raised on unfollow. Downstream notifications (PRD #162) subscribe to this.
/// </summary>
public sealed record UserFollowed(string FollowerId, string FollowedId) : INotification;
