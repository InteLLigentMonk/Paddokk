namespace Paddokk.Core.Models.Entities;

/// <summary>
/// The closed set of in-app Notification types (PRD #162). All five values are defined now;
/// only <see cref="JourneyLiked"/> has a producer wired in this slice — the rest land as thin
/// add-ons in later slices. Direct interactions only; ambient activity lives in the Feed (ADR-0004).
/// </summary>
public enum NotificationType
{
    JourneyLiked,
    CarLiked,
    CommentOnYourPost,
    ReplyToYourComment,
    NewFollower
}
