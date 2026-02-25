using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Paddokk.Core.Models.DTOs.Image;

public class UploadCarImageRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [StringLength(500)]
    public string? Caption { get; set; }
}
