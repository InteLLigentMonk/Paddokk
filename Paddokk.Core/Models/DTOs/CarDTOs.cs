using Paddokk.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs;

// Car Database DTOs
public class CarMakeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public CarGroup Group { get; set; }
    public int ModelCount { get; set; }
}

public class CarModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CarMakeId { get; set; }
    public string CarMakeName { get; set; } = string.Empty;
    public int GenerationCount { get; set; }
}

public class CarGenerationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public int CarModelId { get; set; }
    public string CarModelName { get; set; } = string.Empty;
}

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

public class CreateUserCarRequest
{
    [Required]
    public int CarMakeId { get; set; }

    [Required]
    public int CarModelId { get; set; }

    public int? CarGenerationId { get; set; }

    [Required]
    [Range(1900, 2030)]
    public int Year { get; set; }

    [StringLength(100)]
    public string? Nickname { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public bool IsPrimary { get; set; } = false;
}

public class UpdateUserCarRequest
{
    [StringLength(100)]
    public string? Nickname { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool? IsPrimary { get; set; }
}
