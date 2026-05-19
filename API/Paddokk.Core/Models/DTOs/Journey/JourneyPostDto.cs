namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyPostDto
{
    public required int Id { get; set; }
    public required int JourneyId { get; set; }
    public required string AuthorId { get; set; }
    public required string UserUsername { get; set; }
    public required string UserDisplayName { get; set; }
    public string? UserAvatarUrl { get; set; }

    public string? TextContent { get; set; }
    public required List<JourneyPostImageDto> Images { get; set; }
    public required int CommentCount { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required bool IsEdited { get; set; }
    public required bool IsOwner { get; set; }
}
