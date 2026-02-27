using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Commands.UpdateJourneyPost;

public record UpdateJourneyPostCommand(
    int PostId,
    string? TextContent
) : ICommand<Result<JourneyPostDto>>;
