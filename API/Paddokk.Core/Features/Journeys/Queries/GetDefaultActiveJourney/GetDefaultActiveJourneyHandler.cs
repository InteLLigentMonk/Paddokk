using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetDefaultActiveJourney;

public sealed class GetDefaultActiveJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetDefaultActiveJourneyQuery, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(GetDefaultActiveJourneyQuery request, CancellationToken cancellationToken)
    {
        var user = await journeyRepository.GetUserAsync(actor.UserId, cancellationToken);

        if (user?.DefaultActiveJourneyId is null)
            return Result<JourneyDto>.Failure(Error.NotFound("No default active journey set"));

        var journey = await journeyRepository.GetJourneyByIdAsync(user.DefaultActiveJourneyId.Value, cancellationToken);

        if (journey is null)
            return Result<JourneyDto>.Failure(Error.NotFound("Default active journey not found"));

        return Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(journey, actor.UserId));
    }
}
