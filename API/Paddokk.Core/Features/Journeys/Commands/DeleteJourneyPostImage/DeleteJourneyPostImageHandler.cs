using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPostImage;

public sealed class DeleteJourneyPostImageHandler(
    IJourneyRepository journeyRepository,
    IImageService imageService,
    IActorResolver actor)
    : IRequestHandler<DeleteJourneyPostImageCommand, Result>
{
    public async Task<Result> Handle(DeleteJourneyPostImageCommand command, CancellationToken ct)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(command.JourneyId, ct);

        if (journey is null || journey.UserId != actor.UserId)
            return Result.Failure(Error.NotFound("Journey not found or you don't own it"));

        await imageService.DeleteImageAsync(command.ImageUrl, ct);

        return Result.Success();
    }
}
