using API.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs;

public class ImageUploadDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string MediumUrl { get; set; } = string.Empty;
    public string FullUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class ImageUploadRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [StringLength(500)]
    public string? Caption { get; set; }

    [Required]
    public ImageContext Context { get; set; }

    public int? ContextId { get; set; } // CarId or PostId
}

public class CarImageDto
{
    public int Id { get; set; }
    public int UserCarId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string MediumUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateCarImageRequest
{
    [StringLength(500)]
    public string? Caption { get; set; }

    public int? SortOrder { get; set; }
    public bool? IsPrimary { get; set; }
}

public class ImageLimitsDto
{
    public int MaxImagesPerPost { get; set; }
    public int MaxImagesPerCar { get; set; }
    public long MaxFileSizeBytes { get; set; }
    public string[] AllowedFormats { get; set; } = Array.Empty<string>();
    public SubscriptionTier SubscriptionTier { get; set; }
}

public enum ImageContext
{
    Car = 1,
    JourneyPost = 2,
    UserAvatar = 3
}
