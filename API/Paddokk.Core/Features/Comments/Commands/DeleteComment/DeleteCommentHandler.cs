using MediatR;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Comments.Commands.DeleteComment;

public sealed class DeleteCommentHandler(
    ICommentRepository comments,
    IActorResolver actor,
    ILogger<DeleteCommentHandler> logger) : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand command, CancellationToken ct)
    {
        var comment = await comments.GetCommentByIdAsync(command.CommentId, ct);

        if (comment is null)
            return Result.Failure(Error.NotFound("Comment not found"));

        var canDelete = comment.UserId == actor.UserId
                        || comment.JourneyPost.UserId == actor.UserId;

        if (!canDelete)
            return Result.Failure(Error.Unauthorized("Cannot delete this comment"));

        await comments.DeleteCommentAsync(comment, ct);

        logger.LogInformation("Comment {CommentId} deleted by user {UserId}", command.CommentId, actor.UserId);

        return Result.Success();
    }
}
