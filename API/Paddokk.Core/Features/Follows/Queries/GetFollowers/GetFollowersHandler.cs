using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Users;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Follows.Queries.GetFollowers;

public sealed class GetFollowersHandler(IUserFollowRepository follows, IActorResolver actor)
    : IRequestHandler<GetFollowersQuery, Result<PagedResult<UserDto>>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(GetFollowersQuery query, CancellationToken ct)
    {
        var actorUserId = actor.IsAuthenticated ? actor.UserId : null;

        var (items, total) = await follows.GetFollowersAsync(query.UserId, actorUserId, query.Page, query.PageSize, ct);

        var dtos = items.Select(UserMapping.ToDto).ToList();

        return Result<PagedResult<UserDto>>.Success(
            PagedResult<UserDto>.Create(dtos, total, query.Page, query.PageSize));
    }
}
