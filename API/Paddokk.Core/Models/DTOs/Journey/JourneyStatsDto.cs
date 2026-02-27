namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyStatsDto
{
    public required int TotalJourneys { get; set; }
    public required int ActiveJourneys { get; set; }
    public required int CompletedJourneys { get; set; }
    public required int TotalPosts { get; set; }
    public required int TotalSubscribers { get; set; }
    public required int TotalLikes { get; set; }
}
