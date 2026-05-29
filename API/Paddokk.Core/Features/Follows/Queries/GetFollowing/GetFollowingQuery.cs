using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Follows.Queries.GetFollowing;

public record GetFollowingQuery(string UserId, int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<Result<PagedResult<UserDto>>>;
