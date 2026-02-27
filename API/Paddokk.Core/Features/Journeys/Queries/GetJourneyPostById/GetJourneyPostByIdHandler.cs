using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPostById;

public sealed class GetJourneyPostByIdHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetJourneyPostByIdQuery, Result<JourneyPostDto>>
{
    public async Task<Result<JourneyPostDto>> Handle(GetJourneyPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await journeyRepository.GetJourneyPostByIdAsync(request.PostId, cancellationToken);

        if (post is null)
            return Result<JourneyPostDto>.Failure(Error.NotFound($"Post {request.PostId} not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        return Result<JourneyPostDto>.Success(JourneyMapping.ToJourneyPostDto(post, currentUserId));
    }
}
