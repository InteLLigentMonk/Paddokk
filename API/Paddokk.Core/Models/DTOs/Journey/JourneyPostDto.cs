namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyPostDto
{
    public int Id { get; set; }
    public int JourneyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }

    public string? TextContent { get; set; }
    public List<JourneyPostImageDto> Images { get; set; } = new();
    public int CommentCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsOwner { get; set; } // For current user
}
