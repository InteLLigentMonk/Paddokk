using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Features.Journeys;

namespace Paddokk.Core.Features.Journeys.Queries.GetCarJourneys;

public sealed class GetCarJourneysHandler(
    IJourneyRepository journeyRepository,
    IActorResolver actor)
    : IRequestHandler<GetCarJourneysQuery, Result<IEnumerable<JourneyDto>>>
{
    public async Task<Result<IEnumerable<JourneyDto>>> Handle(
        GetCarJourneysQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var journeys = await journeyRepository.GetCarJourneysAsync(
            request.Username,
            request.CarSlug,
            currentUserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Result<IEnumerable<JourneyDto>>.Success(
            journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId)));
    }
}
