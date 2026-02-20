namespace Paddokk.Core.Models.DTOs.Comment;

public class PostCommentDto
{
    public int Id { get; set; }
    public int JourneyPostId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsOwner { get; set; } // True if current user owns this comment
    public bool IsPostOwner { get; set; } // True if commenter is the post owner
}
