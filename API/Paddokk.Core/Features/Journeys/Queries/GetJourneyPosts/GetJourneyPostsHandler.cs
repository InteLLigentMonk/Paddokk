using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;

public sealed class GetJourneyPostsHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetJourneyPostsQuery, Result<IEnumerable<JourneyPostDto>>>
{
    public async Task<Result<IEnumerable<JourneyPostDto>>> Handle(GetJourneyPostsQuery request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
            return Result<IEnumerable<JourneyPostDto>>.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        if (!journey.IsPublic && currentUserId != journey.PrincipalId)
            return Result<IEnumerable<JourneyPostDto>>.Success([]);

        var posts = await journeyRepository.GetJourneyPostsAsync(request.JourneyId, request.Skip, request.Take, cancellationToken);
        return Result<IEnumerable<JourneyPostDto>>.Success(posts.Select(p => JourneyMapping.ToJourneyPostDto(p, currentUserId)));
    }
}
