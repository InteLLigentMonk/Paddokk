namespace Paddokk.Core.Models.DTOs.Image;

public class CarImageDto
{
    public required int Id { get; set; }
    public required int UserCarId { get; set; }
    public required string ImageUrl { get; set; }
    public required string ThumbnailUrl { get; set; }
    public required string MediumUrl { get; set; }
    public string? Caption { get; set; }
    public required int SortOrder { get; set; }
    public required bool IsPrimary { get; set; }
    public required DateTime CreatedAt { get; set; }
}
