using MediatR;

namespace Paddokk.Core.Features.Cars.Events;

/// <summary>
/// Raised after a User Likes another User's UserCar (not on idempotent re-like, not on self-like
/// which is rejected upstream). The Notifications module subscribes to this to create a
/// <c>CarLiked</c> Notification for the UserCar owner (PRD #162).
/// </summary>
public sealed record CarLiked(string ActorId, int UserCarId, string CarOwnerId) : INotification;
