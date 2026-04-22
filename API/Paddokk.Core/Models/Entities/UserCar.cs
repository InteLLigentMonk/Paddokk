using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.Entities;

public class UserCar
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // Car Details — null when IsCustomBuild = true
    public int? CarMakeId { get; set; }
    public CarMake? CarMake { get; set; }
    public int? CarModelId { get; set; }
    public CarModel? CarModel { get; set; }
    public int? CarGenerationId { get; set; }
    public CarGeneration? CarGeneration { get; set; }
    public int? Year { get; set; }

    // Custom build
    public bool IsCustomBuild { get; set; }

    [StringLength(200)]
    public string? CustomBuildName { get; set; }

    // User Customization
    [StringLength(100)]
    public string? Nickname { get; set; } // "Drift Missile", "Daily"

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(10000)]
    public string? Description { get; set; }

    public string? PrimaryImageUrl { get; set; }
    public bool IsPrimary { get; set; }

    // Full-text search (SearchVector is a shadow property managed by EF/Npgsql in the Data layer)
    public string? SearchText { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Journey> Journeys { get; set; } = [];
    public ICollection<UserCarImage> Images { get; set; } = [];
    public ICollection<UserCarLike> Likes { get; set; } = [];
    public ICollection<UserCarSubscription> Subscriptions { get; set; } = [];
}

public class UserCarImage
{
    public int Id { get; set; }
    public int UserCarId { get; set; }
    public UserCar UserCar { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string MediumUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    public long FileSizeBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string ContentType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}