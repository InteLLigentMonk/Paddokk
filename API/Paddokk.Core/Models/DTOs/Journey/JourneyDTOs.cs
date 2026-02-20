using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JourneyCategory Category { get; set; }
    public JourneyStatus Status { get; set; }

    // User Info
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }

    // Car Info
    public int UserCarId { get; set; }
    public string CarMakeName { get; set; } = string.Empty;
    public string CarModelName { get; set; } = string.Empty;
    public string? CarGenerationName { get; set; }
    public int CarYear { get; set; }
    public string? CarNickname { get; set; }
    public string? CarPrimaryImageUrl { get; set; }

    // Journey Stats
    public int PostCount { get; set; }
    public int SubscriberCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsSubscribed { get; set; } // For current user
    public bool IsLiked { get; set; } // For current user
    public bool IsOwner { get; set; } // For current user

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Latest Post Preview
    public DateTime? LastPostAt { get; set; }
    public string? LastPostPreview { get; set; }
}