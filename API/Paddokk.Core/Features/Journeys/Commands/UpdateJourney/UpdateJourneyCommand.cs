using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.UpdateJourney;

public record UpdateJourneyCommand(
    int JourneyId,
    string? Title,
    string? Description,
    JourneyCategory? Category,
    JourneyStatus? Status,
    DateTime? CompletedAt
) : ICommand<Result<JourneyDto>>;
