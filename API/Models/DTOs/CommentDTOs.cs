using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs;

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

public class CreateCommentRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}

public class UpdateCommentRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}

public class CommentsPagedResponse
{
    public IEnumerable<PostCommentDto> Comments { get; set; } = new List<PostCommentDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
