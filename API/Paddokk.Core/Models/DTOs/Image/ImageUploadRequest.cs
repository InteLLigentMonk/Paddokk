using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Image;

public class ImageUploadRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [StringLength(500)]
    public string? Caption { get; set; }

    [Required]
    public ImageContext Context { get; set; }

    public int? ContextId { get; set; } // CarId or PostId
}
