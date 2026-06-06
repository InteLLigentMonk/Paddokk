using MediatR;

namespace Paddokk.Core.Features.Comments.Events;

/// <summary>
/// Raised after a Reply (<c>ParentCommentId IS NOT NULL</c>, per ADR-0002) is created on a
/// JourneyPost. Only the JourneyPost owner can author a Reply — the validator enforces that
/// invariant, so this event trusts it. The Notifications module subscribes to create a
/// <c>ReplyToYourComment</c> Notification for the parent Comment's author (PRD #162).
/// Self-Replies are allowed; suppression is the subscriber's job.
/// </summary>
public sealed record RepliedToComment(string ActorId, int PostId, string ParentCommentAuthorId) : INotification;
