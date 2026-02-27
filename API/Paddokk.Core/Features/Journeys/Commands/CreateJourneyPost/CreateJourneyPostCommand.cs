using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourneyPost;

public record CreateJourneyPostCommand(
    int JourneyId,
    string? TextContent,
    List<CreateJourneyPostImageRequest> Images
) : ICommand<Result<JourneyPostDto>>;
