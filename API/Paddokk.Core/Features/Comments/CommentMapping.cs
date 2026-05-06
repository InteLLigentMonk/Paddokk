using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Comments;

internal static class CommentMapping
{
    internal static PostCommentDto ToDto(PostComment comment, string? currentUserId)
    {
        var postOwnerUserId = comment.JourneyPost.UserId;
        return new PostCommentDto
        {
            Id = comment.Id,
            JourneyPostId = comment.JourneyPostId,
            UserId = comment.UserId,
            UserDisplayName = comment.User.DisplayName,
            UserAvatarUrl = comment.User.AvatarUrl,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited,
            IsOwner = comment.UserId == currentUserId,
            IsPostOwner = comment.UserId == postOwnerUserId,
            IsViewerPostOwner = currentUserId == postOwnerUserId,
            Reply = comment.Replies.FirstOrDefault() is PostComment reply
                ? ToReplyDto(reply, currentUserId, postOwnerUserId)
                : null
        };
    }

    private static PostCommentDto ToReplyDto(PostComment reply, string? currentUserId, string postOwnerUserId) => new()
    {
        Id = reply.Id,
        JourneyPostId = reply.JourneyPostId,
        UserId = reply.UserId,
        UserDisplayName = reply.User.DisplayName,
        UserAvatarUrl = reply.User.AvatarUrl,
        Content = reply.Content,
        CreatedAt = reply.CreatedAt,
        UpdatedAt = reply.UpdatedAt,
        IsEdited = reply.IsEdited,
        IsOwner = reply.UserId == currentUserId,
        IsPostOwner = true,
        IsViewerPostOwner = currentUserId == postOwnerUserId,
        Reply = null
    };
}
