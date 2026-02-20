using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Journey;

public class CreateJourneyPostImageRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }
}
