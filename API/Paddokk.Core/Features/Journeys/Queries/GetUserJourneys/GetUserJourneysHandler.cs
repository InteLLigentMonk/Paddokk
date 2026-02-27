using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneys;

public sealed class GetUserJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetUserJourneysQuery, IEnumerable<JourneyDto>>
{
    public async Task<IEnumerable<JourneyDto>> Handle(GetUserJourneysQuery request, CancellationToken cancellationToken)
    {
        var journeys = await journeyRepository.GetUserJourneysAsync(actor.UserId, cancellationToken);
        return journeys.Select(j => JourneyMapping.ToJourneyDto(j, actor.UserId));
    }
}
