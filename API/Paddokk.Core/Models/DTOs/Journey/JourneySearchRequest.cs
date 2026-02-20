using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneySearchRequest
{
    public string? Query { get; set; }
    public JourneyCategory? Category { get; set; }
    public CarGroup? CarGroup { get; set; }
    public int? CarMakeId { get; set; }
    public int? CarModelId { get; set; }
    public JourneyStatus? Status { get; set; }
    public JourneySortBy SortBy { get; set; } = JourneySortBy.RecentActivity;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}
