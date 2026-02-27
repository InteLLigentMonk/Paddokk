using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyById;

public sealed class GetJourneyByIdHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetJourneyByIdQuery, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(GetJourneyByIdQuery request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null)
            return Result<JourneyDto>.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        return Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(journey, currentUserId));
    }
}
