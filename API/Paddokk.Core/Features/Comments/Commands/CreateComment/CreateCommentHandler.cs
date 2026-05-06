using MediatR;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Comments.Commands.CreateComment;

public sealed class CreateCommentHandler(
    ICommentRepository comments,
    IActorResolver actor,
    ILogger<CreateCommentHandler> logger) : IRequestHandler<CreateCommentCommand, Result<PostCommentDto>>
{
    public async Task<Result<PostCommentDto>> Handle(CreateCommentCommand command, CancellationToken ct)
    {
        if (!await comments.JourneyPostExists(command.PostId, ct))
            return Result<PostCommentDto>.Failure(Error.NotFound("Post not found"));

        if (!await comments.UserExist(actor.UserId, ct))
            return Result<PostCommentDto>.Failure(Error.NotFound("User not found"));

        if (command.ParentCommentId is int parentId)
        {
            var parent = await comments.GetCommentByIdAsync(parentId, ct);

            if (parent is null || parent.JourneyPostId != command.PostId)
                return Result<PostCommentDto>.Failure(Error.NotFound("Parent comment not found"));

            if (parent.ParentCommentId is not null)
                return Result<PostCommentDto>.Failure(Error.Validation("Cannot reply to a reply"));

            if (parent.JourneyPost.UserId != actor.UserId)
                return Result<PostCommentDto>.Failure(Error.Unauthorized("Only the post owner can reply"));
        }

        var comment = new PostComment
        {
            JourneyPostId = command.PostId,
            UserId = actor.UserId,
            Content = command.Content.Trim(),
            ParentCommentId = command.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await comments.CreateCommentAsync(comment, ct);

        logger.LogInformation("User {UserId} commented on post {PostId}", actor.UserId, command.PostId);

        var created = await comments.GetCommentByIdAsync(comment.Id, ct);

        return Result<PostCommentDto>.Success(CommentMapping.ToDto(created!, actor.UserId));
    }
}
