using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Queries.GetCommentById;

public sealed class GetCommentByIdHandler(
    ICommentRepository comments,
    IActorResolver actor) : IRequestHandler<GetCommentByIdQuery, Result<PostCommentDto>>
{
    public async Task<Result<PostCommentDto>> Handle(GetCommentByIdQuery query, CancellationToken ct)
    {
        var comment = await comments.GetCommentByIdAsync(query.CommentId, ct);

        if (comment is null)
            return Result<PostCommentDto>.Failure(Error.NotFound("Comment not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        return Result<PostCommentDto>.Success(CommentMapping.ToDto(comment, currentUserId));
    }
}
