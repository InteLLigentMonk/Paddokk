using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Core.Features.Feed.Queries.GetFeed;

public sealed class GetFeedHandler(IFeedRepository feedRepository, IActorResolver actor)
    : IRequestHandler<GetFeedQuery, Result<PagedResult<FeedItemDto>>>
{
    public async Task<Result<PagedResult<FeedItemDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        // Defence in depth: the endpoint carries [Authorize], but the handler must not
        // fall back to an anonymous Feed if that guard is ever absent.
        if (!actor.IsAuthenticated)
            return Result<PagedResult<FeedItemDto>>.Failure(Error.Unauthorized("Authentication is required to view the feed"));

        var (items, totalCount) = await feedRepository.GetFeedAsync(actor.UserId, request.Page, request.PageSize, cancellationToken);

        return Result<PagedResult<FeedItemDto>>.Success(
            PagedResult<FeedItemDto>.Create(items.ToList(), totalCount, request.Page, request.PageSize));
    }
}
