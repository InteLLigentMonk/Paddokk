using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public required JourneyCategory Category { get; set; }
    public required JourneyStatus Status { get; set; }
    public required JourneyActivityTier ActivityTier { get; set; }
    public required bool IsPublic { get; set; }

    // Owner Info
    public required string PrincipalId { get; set; }
    public required string OwnerUsername { get; set; }
    public required string UserDisplayName { get; set; }
    public string? UserAvatarUrl { get; set; }

    // Car Info
    public required int UserCarId { get; set; }
    public required string CarSlug { get; set; }
    public string? CarMakeName { get; set; }
    public string? CarModelName { get; set; }
    public string? CarGenerationName { get; set; }
    public int? CarYear { get; set; }
    public string? CarNickname { get; set; }
    public string? CarPrimaryImageUrl { get; set; }

    // Journey Stats
    public required int PostCount { get; set; }
    public required int SubscriberCount { get; set; }
    public required int LikeCount { get; set; }
    public required bool IsSubscribed { get; set; }
    public required bool IsLiked { get; set; }
    public required bool IsOwner { get; set; }

    // Cover
    public string? PrimaryImageUrl { get; set; }

    // Metadata
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? TargetCompletedAt { get; set; }

    // Latest Post Preview
    public DateTime? LastPostAt { get; set; }
    public string? LastPostPreview { get; set; }
}
