namespace Paddokk.Core.Models.DTOs.Comment;

public class PostCommentDto
{
    public required int Id { get; set; }
    public required int JourneyPostId { get; set; }
    public required string UserId { get; set; }
    public required string UserDisplayName { get; set; }
    public string? UserAvatarUrl { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required bool IsEdited { get; set; }
    public required bool IsOwner { get; set; }
    public required bool IsPostOwner { get; set; }
    public required bool IsViewerPostOwner { get; set; }
    public PostCommentDto? Reply { get; set; }
}
