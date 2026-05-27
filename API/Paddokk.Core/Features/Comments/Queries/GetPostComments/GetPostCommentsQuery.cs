using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Queries.GetPostComments;

public record GetPostCommentsQuery(int PostId, int Page = 1, int PageSize = PaginationDefaults.DefaultPageSize)
    : IQuery<Result<PagedResult<PostCommentDto>>>;
