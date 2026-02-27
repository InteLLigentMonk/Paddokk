using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Paddokk.Core.Models.DTOs.Image;

public class UploadCarImageRequest
{
    [Required]
    public required IFormFile File { get; set; }

    [StringLength(500)]
    public string? Caption { get; set; }
}
