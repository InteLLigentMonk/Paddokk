using MediatR;

namespace Paddokk.Core.Features.Journeys.Events;

/// <summary>
/// Raised after a User Likes another User's Journey (not on idempotent re-like, not on self-like
/// which is rejected upstream). The Notifications module subscribes to this to create a
/// <c>JourneyLiked</c> Notification for the Journey owner (PRD #162).
/// </summary>
public sealed record JourneyLiked(string ActorId, int JourneyId, string JourneyOwnerId) : INotification;
