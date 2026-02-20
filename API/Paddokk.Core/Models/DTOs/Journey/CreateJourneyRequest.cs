using Paddokk.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Journey;

public class CreateJourneyRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public JourneyCategory Category { get; set; }

    [Required]
    public int UserCarId { get; set; }

    public bool SetAsDefaultActive { get; set; } = true;
}
