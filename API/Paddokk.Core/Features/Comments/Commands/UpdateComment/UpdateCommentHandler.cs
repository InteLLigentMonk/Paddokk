using MediatR;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Commands.UpdateComment;

public sealed class UpdateCommentHandler(
    ICommentRepository comments,
    IActorResolver actor,
    ILogger<UpdateCommentHandler> logger) : IRequestHandler<UpdateCommentCommand, Result<PostCommentDto>>
{
    public async Task<Result<PostCommentDto>> Handle(UpdateCommentCommand command, CancellationToken ct)
    {
        var comment = await comments.GetCommentByIdAsync(command.CommentId, ct);

        if (comment is null)
            return Result<PostCommentDto>.Failure(Error.NotFound("Comment not found"));

        if (comment.UserId != actor.UserId)
            return Result<PostCommentDto>.Failure(Error.Unauthorized("Not your comment"));

        var updated = await comments.UpdateCommentAsync(actor.UserId, command.CommentId, command.Content, ct);

        if (!updated)
            return Result<PostCommentDto>.Failure(Error.NotFound("Comment not found"));

        logger.LogInformation("User {UserId} updated comment {CommentId}", actor.UserId, command.CommentId);

        var result = await comments.GetCommentByIdAsync(command.CommentId, ct);

        return Result<PostCommentDto>.Success(CommentMapping.ToDto(result!, actor.UserId));
    }
}
