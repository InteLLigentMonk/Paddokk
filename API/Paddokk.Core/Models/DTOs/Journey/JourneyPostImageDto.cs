namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyPostImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}
