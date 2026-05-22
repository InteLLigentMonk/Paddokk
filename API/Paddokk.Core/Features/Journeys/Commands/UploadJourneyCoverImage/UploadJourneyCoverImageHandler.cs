using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyCoverImage;

public sealed class UploadJourneyCoverImageHandler(
    IJourneyRepository journeyRepository,
    IImageService imageService,
    IActorResolver actor)
    : IRequestHandler<UploadJourneyCoverImageCommand, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(UploadJourneyCoverImageCommand command, CancellationToken ct)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(command.JourneyId, ct);

        if (journey is null || journey.PrincipalId != actor.UserId)
            return Result<JourneyDto>.Failure(Error.NotFound("Journey not found or you don't own it"));

        var uploaded = await imageService.UploadImageAsync(command.File, ImageContext.JourneyCover, ct, command.JourneyId);

        journey.CoverImageUrl = uploaded.ImageUrl;
        journey.UpdatedAt = DateTime.UtcNow;

        await journeyRepository.UpdateJourneyAsync(journey, ct);

        var updated = await journeyRepository.GetJourneyByIdAsync(command.JourneyId, ct);
        return Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(updated!, actor.UserId));
    }
}
