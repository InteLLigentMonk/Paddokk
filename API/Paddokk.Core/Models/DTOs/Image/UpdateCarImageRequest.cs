using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Image;

public class UpdateCarImageRequest
{
    [StringLength(500)]
    public string? Caption { get; set; }

    public int? SortOrder { get; set; }
    public bool? IsPrimary { get; set; }
}
