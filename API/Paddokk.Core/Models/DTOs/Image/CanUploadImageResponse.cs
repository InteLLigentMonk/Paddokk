using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Image;

public class CanUploadImageResponse
{
    public required bool CanUpload { get; set; }
    public required int CurrentCount { get; set; }
    public required int MaxAllowed { get; set; }
    public required SubscriptionTier SubscriptionTier { get; set; }
}
