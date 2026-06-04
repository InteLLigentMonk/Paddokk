using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Core.Features.Feed.Queries.GetFeed;

/// <summary>
/// The personalised, strictly chronological Feed for the authenticated actor. The actor is
/// resolved from <see cref="IActorResolver"/> in the handler, not passed in — so an anonymous
/// caller cannot request another User's Feed by id.
/// </summary>
public record GetFeedQuery(int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<Result<PagedResult<FeedItemDto>>>;
