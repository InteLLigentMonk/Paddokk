using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;

public sealed class GetJourneyPostsHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetJourneyPostsQuery, Result<PagedResult<JourneyPostDto>>>
{
    public async Task<Result<PagedResult<JourneyPostDto>>> Handle(GetJourneyPostsQuery request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
            return Result<PagedResult<JourneyPostDto>>.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;
        if (!journey.IsPublic && currentUserId != journey.PrincipalId)
            return Result<PagedResult<JourneyPostDto>>.Success(PagedResult<JourneyPostDto>.Empty(request.Page, request.PageSize));

        var (posts, totalCount) = await journeyRepository.GetJourneyPostsAsync(request.JourneyId, request.Page, request.PageSize, cancellationToken);
        var items = posts.Select(p => JourneyMapping.ToJourneyPostDto(p, currentUserId)).ToList();

        return Result<PagedResult<JourneyPostDto>>.Success(
            PagedResult<JourneyPostDto>.Create(items, totalCount, request.Page, request.PageSize));
    }
}
