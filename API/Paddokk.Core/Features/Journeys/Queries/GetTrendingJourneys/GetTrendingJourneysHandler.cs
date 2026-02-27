using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetTrendingJourneys;

public sealed class GetTrendingJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetTrendingJourneysQuery, IEnumerable<JourneyDto>>
{
    public async Task<IEnumerable<JourneyDto>> Handle(GetTrendingJourneysQuery request, CancellationToken cancellationToken)
    {
        var search = new JourneySearchRequest { SortBy = JourneySortBy.RecentActivity, Take = 10 };
        var journeys = await journeyRepository.SearchJourneysAsync(search, cancellationToken);
        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        return journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId));
    }
}
