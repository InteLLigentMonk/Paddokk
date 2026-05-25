using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.Entities;

public class ApplicationUser
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? LastName { get; set; }

    [Required]
    [StringLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsernameChangeAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Subscription
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public DateTime? SubscriptionExpiresAt { get; set; }

    public Role Role { get; set; } = Role.User;

    // Settings
    public int? DefaultActiveJourneyId { get; set; }

    // Navigation properties
    public ICollection<UserCar> Cars { get; set; } = [];
    public ICollection<Journey> Journeys { get; set; } = [];
    public ICollection<JourneySubscription> JourneySubscriptions { get; set; } = [];
    public ICollection<JourneyLike> JourneyLikes { get; set; } = [];
    public ICollection<UserCarLike> UserCarLikes { get; set; } = [];
    public ICollection<UserCarSubscription> UserCarSubscriptions { get; set; } = [];
    public ICollection<PostComment> Comments { get; set; } = [];
    public Journey? DefaultActiveJourney { get; set; }
}

public enum SubscriptionTier
{
    Free = 0,
    Silver = 1,
    Gold = 2,
    Platinum = 3,
    Diamond = 4
}

public enum Role
{
    User = 0,
    Moderator = 1,
    Admin = 2
}
