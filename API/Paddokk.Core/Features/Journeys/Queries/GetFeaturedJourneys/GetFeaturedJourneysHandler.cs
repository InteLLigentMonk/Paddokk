using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetFeaturedJourneys;

public sealed class GetFeaturedJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetFeaturedJourneysQuery, IEnumerable<JourneyDto>>
{
    public async Task<IEnumerable<JourneyDto>> Handle(GetFeaturedJourneysQuery request, CancellationToken cancellationToken)
    {
        var search = new JourneySearchRequest { SortBy = JourneySortBy.MostLiked, PageSize = 10 };
        var (journeys, _) = await journeyRepository.SearchJourneysAsync(search, cancellationToken);
        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        return journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId));
    }
}
