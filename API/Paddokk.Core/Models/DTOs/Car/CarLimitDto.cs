namespace Paddokk.Core.Models.DTOs.Car;

public class CarLimitDto
{
    public required bool CanAdd { get; set; }
    public required int CurrentCount { get; set; }
    public required string MaxAllowed { get; set; }
    public required string SubscriptionTier { get; set; }
}
