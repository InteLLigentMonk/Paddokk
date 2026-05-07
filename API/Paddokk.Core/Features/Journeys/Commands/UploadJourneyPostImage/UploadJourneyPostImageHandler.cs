using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyPostImage;

public sealed class UploadJourneyPostImageHandler(
    IJourneyRepository journeyRepository,
    IImageService imageService,
    IActorResolver actor)
    : IRequestHandler<UploadJourneyPostImageCommand, Result<ImageUploadDto>>
{
    public async Task<Result<ImageUploadDto>> Handle(UploadJourneyPostImageCommand command, CancellationToken ct)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(command.JourneyId, ct);

        if (journey is null || journey.PrincipalId != actor.UserId)
            return Result<ImageUploadDto>.Failure(Error.NotFound("Journey not found or you don't own it"));

        var limits = await imageService.GetImageLimitsAsync(actor.UserId, ct);
        if (!await imageService.CanUserUploadImageAsync(actor.UserId, ImageContext.JourneyPost, ct, command.JourneyId))
            return Result<ImageUploadDto>.Failure(Error.Validation($"Image limit reached ({limits.MaxImagesPerPost}) for this subscription tier"));

        var uploaded = await imageService.UploadImageAsync(command.File, ImageContext.JourneyPost, ct, command.JourneyId);

        return Result<ImageUploadDto>.Success(uploaded);
    }
}
