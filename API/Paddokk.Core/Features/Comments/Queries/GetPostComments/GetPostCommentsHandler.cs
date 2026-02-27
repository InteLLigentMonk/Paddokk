using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Queries.GetPostComments;

public sealed class GetPostCommentsHandler(
    ICommentRepository comments,
    IActorResolver actor) : IRequestHandler<GetPostCommentsQuery, Result<CommentsPagedResponse>>
{
    public async Task<Result<CommentsPagedResponse>> Handle(
        GetPostCommentsQuery query, CancellationToken ct)
    {
        if (!await comments.JourneyPostExists(query.PostId, ct))
            return Result<CommentsPagedResponse>.Failure(Error.NotFound("Post not found"));

        var (postComments, total) = await comments.GetPostCommentsAsync(
            query.PostId, ct, query.Page, query.PageSize);

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        return Result<CommentsPagedResponse>.Success(new CommentsPagedResponse
        {
            Comments = postComments.Select(c => CommentMapping.ToDto(c, currentUserId)).ToList(),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize,
            HasNext = total > query.Page * query.PageSize,
            HasPrevious = query.Page > 1
        });
    }
}
