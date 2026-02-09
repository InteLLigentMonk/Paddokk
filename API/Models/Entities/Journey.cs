using System.ComponentModel.DataAnnotations;

namespace API.Models.Entities;

public class Journey
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public JourneyCategory Category { get; set; }
    public JourneyStatus Status { get; set; } = JourneyStatus.Active;

    // Relationships
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int UserCarId { get; set; }
    public UserCar UserCar { get; set; } = null!;

    // Content
    public ICollection<JourneyPost> Posts { get; set; } = new List<JourneyPost>();
    public ICollection<JourneySubscription> Subscriptions { get; set; } = new List<JourneySubscription>();
    public ICollection<JourneyLike> Likes { get; set; } = new List<JourneyLike>();

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Computed Properties (not mapped to database)
    public int PostCount => Posts?.Count ?? 0;
    public int SubscriberCount => Subscriptions?.Count(s => s.IsActive) ?? 0;
    public int LikeCount => Likes?.Count ?? 0;
}

public enum JourneyCategory
{
    BuildAndMods = 1,
    Restoration = 2,
    Racing = 3,
    Adventures = 4,
    Ownership = 5
}

public enum JourneyStatus
{
    Active = 1,
    Completed = 2,
    OnHold = 3,
    Archived = 4
}
