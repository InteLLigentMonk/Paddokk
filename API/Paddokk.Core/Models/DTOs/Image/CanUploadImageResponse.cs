using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Image;

public class CanUploadImageResponse
{
    public bool CanUpload { get; set; }

    public int CurrentCount { get; set; }

    public int MaxAllowed { get; set; }

    public SubscriptionTier SubscriptionTier { get; set; }
}
