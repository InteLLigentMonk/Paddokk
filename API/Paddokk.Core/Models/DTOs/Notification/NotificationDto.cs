using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Notification;

/// <summary>
/// A single Notification as seen by its recipient. Actor display fields and <see cref="TargetUrl"/>
/// are resolved on read (ADR-0003) — never stored on the row — so renames, avatar changes and
/// GDPR anonymization are reflected for free. <see cref="TargetUrl"/> is a ready-to-navigate
/// relative path; the client does not need to know how to route each <see cref="EntityType"/>.
/// </summary>
public record NotificationDto
{
    public required int Id { get; init; }
    public required NotificationType Type { get; init; }

    // Polymorphic source reference, retained so the client can render type-specific copy.
    public required string EntityType { get; init; }
    public required string EntityId { get; init; }

    /// <summary>Relative deep-link to the source, resolved server-side. Empty when unresolvable.</summary>
    public required string TargetUrl { get; init; }

    public required bool Read { get; init; }
    public required DateTime CreatedAt { get; init; }

    // Actor — joined from ApplicationUser on read, reflecting current state.
    public required string ActorId { get; init; }
    public required string ActorUsername { get; init; }
    public required string ActorDisplayName { get; init; }
    public string? ActorAvatarUrl { get; init; }
}
