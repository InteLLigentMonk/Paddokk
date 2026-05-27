using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;

public record GetJourneyPostsQuery(int JourneyId, int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<Result<PagedResult<JourneyPostDto>>>;
