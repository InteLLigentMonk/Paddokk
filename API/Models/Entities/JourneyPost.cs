using System.ComponentModel.DataAnnotations;

namespace API.Models.Entities;

public class JourneyPost
{
    public int Id { get; set; }
    public int JourneyId { get; set; }
    public Journey Journey { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // Content
    [StringLength(5000)]
    public string? TextContent { get; set; }

    public ICollection<JourneyPostImage> Images { get; set; } = new List<JourneyPostImage>();
    public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; } = false;

    // Computed
    public int CommentCount => Comments?.Count ?? 0;
}

public class JourneyPostImage
{
    public int Id { get; set; }
    public int JourneyPostId { get; set; }
    public JourneyPost JourneyPost { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
