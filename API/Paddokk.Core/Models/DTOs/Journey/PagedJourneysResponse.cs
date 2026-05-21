namespace Paddokk.Core.Models.DTOs.Journey;

public class PagedJourneysResponse
{
    public required List<JourneyDto> Journeys { get; init; }
    public required int TotalCount { get; init; }
    public required bool HasMore { get; init; }
}
