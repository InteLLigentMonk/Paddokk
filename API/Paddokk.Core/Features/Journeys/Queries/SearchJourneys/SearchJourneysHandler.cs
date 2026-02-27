using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.SearchJourneys;

public sealed class SearchJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<SearchJourneysQuery, IEnumerable<JourneyDto>>
{
    public async Task<IEnumerable<JourneyDto>> Handle(SearchJourneysQuery request, CancellationToken cancellationToken)
    {
        var journeys = await journeyRepository.SearchJourneysAsync(request.Search, cancellationToken);
        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        return journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId));
    }
}
