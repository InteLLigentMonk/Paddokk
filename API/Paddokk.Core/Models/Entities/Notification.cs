namespace Paddokk.Core.Models.Entities;

/// <summary>
/// A single in-app Notification row. Stores only <see cref="ActorId"/> — actor display fields
/// (username / displayName / avatar) are joined from <see cref="ApplicationUser"/> on read,
/// never snapshotted (ADR-0003). <see cref="EntityType"/> + <see cref="EntityId"/> are polymorphic
/// so any source can be deep-linked without per-type columns.
/// </summary>
public class Notification
{
    public int Id { get; set; }

    // The user who receives the notification.
    public string RecipientId { get; set; } = string.Empty;
    public ApplicationUser Recipient { get; set; } = null!;

    // The user whose action produced the notification.
    public string ActorId { get; set; } = string.Empty;
    public ApplicationUser Actor { get; set; } = null!;

    public NotificationType Type { get; set; }

    // Polymorphic deep-link target, e.g. ("Journey", "42") or ("User", "abc"). EntityId is a string
    // because source ids are heterogeneous (user ids are strings, others are stringified ints).
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;

    public bool Read { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
