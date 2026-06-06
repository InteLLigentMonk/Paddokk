using MediatR;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Features.Comments.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Comments.Commands.CreateComment;

public sealed class CreateCommentHandler(
    ICommentRepository comments,
    IActorResolver actor,
    IPublisher publisher,
    ILogger<CreateCommentHandler> logger) : IRequestHandler<CreateCommentCommand, Result<PostCommentDto>>
{
    public async Task<Result<PostCommentDto>> Handle(CreateCommentCommand command, CancellationToken ct)
    {
        if (!await comments.JourneyPostExists(command.PostId, ct))
            return Result<PostCommentDto>.Failure(Error.NotFound("Post not found"));

        if (!await comments.UserExist(actor.UserId, ct))
            return Result<PostCommentDto>.Failure(Error.NotFound("User not found"));

        string? parentCommentAuthorId = null;

        if (command.ParentCommentId is int parentId)
        {
            var parent = await comments.GetCommentByIdAsync(parentId, ct);

            if (parent is null || parent.JourneyPostId != command.PostId)
                return Result<PostCommentDto>.Failure(Error.NotFound("Parent comment not found"));

            if (parent.ParentCommentId is not null)
                return Result<PostCommentDto>.Failure(Error.Validation("Cannot reply to a reply"));

            if (parent.JourneyPost.AuthorId != actor.UserId)
                return Result<PostCommentDto>.Failure(Error.Unauthorized("Only the post owner can reply"));

            parentCommentAuthorId = parent.AuthorId;
        }

        var comment = new PostComment
        {
            JourneyPostId = command.PostId,
            AuthorId = actor.UserId,
            Content = command.Content.Trim(),
            ParentCommentId = command.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await comments.CreateCommentAsync(comment, ct);

        logger.LogInformation("User {UserId} commented on post {PostId}", actor.UserId, command.PostId);

        var created = await comments.GetCommentByIdAsync(comment.Id, ct);

        // Top-level Comments notify the post author (CommentOnYourPost); Replies notify the parent
        // Comment's author (ReplyToYourComment). Exactly one of the two fires per create (ADR-0002).
        if (command.ParentCommentId is null)
            await publisher.Publish(
                new CommentedOnPost(actor.UserId, command.PostId, created!.JourneyPost.AuthorId),
                ct);
        else
            await publisher.Publish(
                new RepliedToComment(actor.UserId, command.PostId, parentCommentAuthorId!),
                ct);

        return Result<PostCommentDto>.Success(CommentMapping.ToDto(created!, actor.UserId));
    }
}
