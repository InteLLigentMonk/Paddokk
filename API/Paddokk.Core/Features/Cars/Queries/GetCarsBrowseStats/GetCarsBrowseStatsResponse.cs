namespace Paddokk.Core.Features.Cars.Queries.GetCarsBrowseStats;

public class GetCarsBrowseStatsResponse
{
    public int Cars { get; init; }
    public int Makes { get; init; }
    public int Owners { get; init; }
    public int Journeys { get; init; }
}
