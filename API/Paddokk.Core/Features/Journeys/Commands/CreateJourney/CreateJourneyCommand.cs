using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourney;

public record CreateJourneyCommand(
    string Title,
    string? Description,
    JourneyCategory Category,
    int UserCarId,
    bool SetAsDefaultActive = true
) : ICommand<Result<JourneyDto>>
{
    public DateTime? TargetCompletedAt { get; init; }
    public string? CoverImageUrl { get; init; }
}
