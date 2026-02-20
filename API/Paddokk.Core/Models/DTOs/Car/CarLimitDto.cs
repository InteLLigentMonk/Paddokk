namespace Paddokk.Core.Models.DTOs.Car;

public class CarLimitDto
{
    public bool CanAdd { get; set; }
    public int CurrentCount { get; set; }
    public string MaxAllowed { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
}
