using MediatR;

namespace Paddokk.Core.Features.Comments.Events;

/// <summary>
/// Raised after a top-level Comment (<c>ParentCommentId IS NULL</c>, per ADR-0002) is created on a
/// JourneyPost. Replies do not raise this — they belong to the separate ReplyToYourComment flow.
/// The Notifications module subscribes to create a <c>CommentOnYourPost</c> Notification for the
/// JourneyPost author (PRD #162). Self-Comments are allowed; suppression is the subscriber's job.
/// </summary>
public sealed record CommentedOnPost(string ActorId, int PostId, string PostAuthorId) : INotification;
