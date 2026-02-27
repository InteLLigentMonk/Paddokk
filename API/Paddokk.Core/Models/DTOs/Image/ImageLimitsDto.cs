using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Image;

public class ImageLimitsDto
{
    public required int MaxImagesPerPost { get; set; }
    public required int MaxImagesPerCar { get; set; }
    public required long MaxFileSizeBytes { get; set; }
    public required string[] AllowedFormats { get; set; }
    public required SubscriptionTier SubscriptionTier { get; set; }
}
