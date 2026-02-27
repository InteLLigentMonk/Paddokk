namespace Paddokk.Core.Models.DTOs.Journey;

public class JourneyPostImageDto
{
    public required int Id { get; set; }
    public required string ImageUrl { get; set; }
    public string? Caption { get; set; }
    public required int SortOrder { get; set; }
}
