using MediatR;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Queries.GetPostComments;

public sealed class GetPostCommentsHandler(
    ICommentRepository comments,
    IActorResolver actor) : IRequestHandler<GetPostCommentsQuery, Result<PagedResult<PostCommentDto>>>
{
    public async Task<Result<PagedResult<PostCommentDto>>> Handle(
        GetPostCommentsQuery query, CancellationToken ct)
    {
        var parentJourney = await comments.GetParentJourneyAsync(query.PostId, ct);
        if (parentJourney is null)
            return Result<PagedResult<PostCommentDto>>.Failure(Error.NotFound("Post not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        if (!parentJourney.IsPublic && currentUserId != parentJourney.PrincipalId)
        {
            return Result<PagedResult<PostCommentDto>>.Success(
                PagedResult<PostCommentDto>.Empty(query.Page, query.PageSize));
        }

        var (postComments, total) = await comments.GetPostCommentsAsync(
            query.PostId, ct, query.Page, query.PageSize);

        var items = postComments.Select(c => CommentMapping.ToDto(c, currentUserId)).ToList();

        return Result<PagedResult<PostCommentDto>>.Success(
            PagedResult<PostCommentDto>.Create(items, total, query.Page, query.PageSize));
    }
}
