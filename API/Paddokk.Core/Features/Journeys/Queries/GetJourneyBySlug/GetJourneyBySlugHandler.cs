using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyBySlug;

public sealed class GetJourneyBySlugHandler(
    IJourneyRepository journeyRepository,
    IUserRepository userRepository,
    IActorResolver actor)
    : IRequestHandler<GetJourneyBySlugQuery, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(GetJourneyBySlugQuery request, CancellationToken cancellationToken)
    {
        var owner = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (owner is null)
            return Result<JourneyDto>.Failure(Error.NotFound($"User '{request.Username}' not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var journey = await journeyRepository.GetJourneyBySlugAsync(
            request.Username, request.Slug, currentUserId, cancellationToken);

        if (journey is null)
            return Result<JourneyDto>.Failure(Error.NotFound($"Journey '{request.Slug}' not found"));

        return Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(journey, currentUserId));
    }
}
