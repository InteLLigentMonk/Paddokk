using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.SearchJourneys;

public sealed class SearchJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<SearchJourneysQuery, Result<PagedJourneysResponse>>
{
    public async Task<Result<PagedJourneysResponse>> Handle(SearchJourneysQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var (journeys, total) = await journeyRepository.SearchJourneysAsync(request.Search, cancellationToken);

        var hasMore = (long)request.Search.Page * request.Search.PageSize < total;

        return Result<PagedJourneysResponse>.Success(new PagedJourneysResponse
        {
            Journeys = [.. journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId))],
            TotalCount = total,
            HasMore = hasMore
        });
    }
}
