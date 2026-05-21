using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneysBrowseStats;

public sealed class GetJourneysBrowseStatsHandler(IJourneyRepository journeyRepository)
    : IRequestHandler<GetJourneysBrowseStatsQuery, Result<GetJourneysBrowseStatsResponse>>
{
    public async Task<Result<GetJourneysBrowseStatsResponse>> Handle(GetJourneysBrowseStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await journeyRepository.GetBrowseStatsAsync(request.Search, cancellationToken);
        return Result<GetJourneysBrowseStatsResponse>.Success(stats);
    }
}
