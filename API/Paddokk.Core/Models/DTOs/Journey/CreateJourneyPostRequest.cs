using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Journey;

public class CreateJourneyPostRequest
{
    [StringLength(5000)]
    public string? TextContent { get; set; }

    public List<CreateJourneyPostImageRequest> Images { get; set; } = new();
}
