using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneySearchRequest
{
    public List<string> Terms { get; set; } = [];
    public JourneyCategory? Category { get; set; }
    public CarGroup? CarGroup { get; set; }
    public int? CarMakeId { get; set; }
    public int? CarModelId { get; set; }
    public JourneyStatus? Status { get; set; }
    public JourneySortBy SortBy { get; set; } = JourneySortBy.RecentActivity;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 24;
}
