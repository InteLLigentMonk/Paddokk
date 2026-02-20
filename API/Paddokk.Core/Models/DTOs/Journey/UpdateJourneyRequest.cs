using Paddokk.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Journey;

public class UpdateJourneyRequest
{
    [StringLength(200, MinimumLength = 3)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public JourneyCategory? Category { get; set; }
    public JourneyStatus? Status { get; set; }
    public DateTime? CompletedAt { get; set; }
}
