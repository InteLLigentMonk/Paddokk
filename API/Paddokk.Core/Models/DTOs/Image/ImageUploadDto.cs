namespace Paddokk.Core.Models.DTOs.Image;

public class ImageUploadDto
{
    public required string ImageUrl { get; set; }
    public required string FileName { get; set; }
    public required long FileSizeBytes { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public required string ContentType { get; set; }
    public required DateTime UploadedAt { get; set; }
}
