using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Comments;

internal static class CommentMapping
{
    internal static PostCommentDto ToDto(PostComment comment, string? currentUserId)
    {
        var postAuthorId = comment.JourneyPost.AuthorId;
        return new PostCommentDto
        {
            Id = comment.Id,
            JourneyPostId = comment.JourneyPostId,
            AuthorId = comment.AuthorId,
            UserUsername = comment.Author.Username,
            UserDisplayName = comment.Author.DisplayName,
            UserAvatarUrl = comment.Author.AvatarUrl,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited,
            IsOwner = comment.AuthorId == currentUserId,
            IsPostOwner = comment.AuthorId == postAuthorId,
            IsViewerPostOwner = currentUserId == postAuthorId,
            Reply = comment.Replies.FirstOrDefault() is PostComment reply
                ? ToReplyDto(reply, currentUserId, postAuthorId)
                : null
        };
    }

    private static PostCommentDto ToReplyDto(PostComment reply, string? currentUserId, string postAuthorId) => new()
    {
        Id = reply.Id,
        JourneyPostId = reply.JourneyPostId,
        AuthorId = reply.AuthorId,
        UserUsername = reply.Author.Username,
        UserDisplayName = reply.Author.DisplayName,
        UserAvatarUrl = reply.Author.AvatarUrl,
        Content = reply.Content,
        CreatedAt = reply.CreatedAt,
        UpdatedAt = reply.UpdatedAt,
        IsEdited = reply.IsEdited,
        IsOwner = reply.AuthorId == currentUserId,
        IsPostOwner = true,
        IsViewerPostOwner = currentUserId == postAuthorId,
        Reply = null
    };
}
