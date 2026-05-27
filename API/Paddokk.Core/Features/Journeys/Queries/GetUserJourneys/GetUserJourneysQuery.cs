using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneys;

public record GetUserJourneysQuery(int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<PagedResult<JourneyDto>>;
