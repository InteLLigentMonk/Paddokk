namespace Paddokk.Core.Models.DTOs.Car;

// User Car DTOs
public class UserCarDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Car Details
    public int CarMakeId { get; set; }
    public string CarMakeName { get; set; } = string.Empty;
    public int CarModelId { get; set; }
    public string CarModelName { get; set; } = string.Empty;
    public int? CarGenerationId { get; set; }
    public string? CarGenerationName { get; set; }
    public int Year { get; set; }

    // User Customization
    public string? Nickname { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public bool IsPrimary { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Computed
    public int JourneyCount { get; set; }
}
