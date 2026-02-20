using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Image;

public class ImageLimitsDto
{
    public int MaxImagesPerPost { get; set; }
    public int MaxImagesPerCar { get; set; }
    public long MaxFileSizeBytes { get; set; }
    public string[] AllowedFormats { get; set; } = Array.Empty<string>();
    public SubscriptionTier SubscriptionTier { get; set; }
}
