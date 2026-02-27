using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Comments;

internal static class CommentMapping
{
    internal static PostCommentDto ToDto(PostComment comment, string? currentUserId) => new()
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
        IsPostOwner = comment.UserId == comment.JourneyPost.UserId
    };
}
