using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneys;

public sealed class GetUserJourneysHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetUserJourneysQuery, PagedResult<JourneyDto>>
{
    public async Task<PagedResult<JourneyDto>> Handle(GetUserJourneysQuery request, CancellationToken cancellationToken)
    {
        var (journeys, totalCount) = await journeyRepository.GetUserJourneysPagedAsync(
            actor.UserId, request.Page, request.PageSize, cancellationToken);

        var items = journeys.Select(j => JourneyMapping.ToJourneyDto(j, actor.UserId)).ToList();

        return PagedResult<JourneyDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
