namespace Paddokk.Core.Models.DTOs.Car;

public class UserCarDto
{
    public required int Id { get; set; }
    public required string UserId { get; set; }

    // Car Details
    public int? CarMakeId { get; set; }
    public string? CarMakeName { get; set; }
    public int? CarModelId { get; set; }
    public string? CarModelName { get; set; }
    public int? CarGenerationId { get; set; }
    public string? CarGenerationName { get; set; }
    public int? Year { get; set; }

    // Custom build
    public required bool IsCustomBuild { get; set; }
    public string? CustomBuildName { get; set; }

    // User Customization
    public string? Nickname { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public required bool IsPrimary { get; set; }

    // Metadata
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

    // Computed
    public required int JourneyCount { get; set; }
    public required int LikeCount { get; set; }
    public required int SubscriberCount { get; set; }
    public required bool IsLiked { get; set; }
    public required bool IsSubscribed { get; set; }
    public required bool IsOwner { get; set; }
}
