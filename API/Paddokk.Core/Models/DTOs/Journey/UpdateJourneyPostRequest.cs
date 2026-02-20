using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Journey;

public class UpdateJourneyPostRequest
{
    [StringLength(5000)]
    public string? TextContent { get; set; }
}
