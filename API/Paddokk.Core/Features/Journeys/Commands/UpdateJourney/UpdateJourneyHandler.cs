using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.UpdateJourney;

public sealed class UpdateJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<UpdateJourneyCommand, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null || journey.UserId != actor.UserId)
            return Result<JourneyDto>.Failure(Error.NotFound("Journey not found or you don't own it"));

        if (!string.IsNullOrEmpty(request.Title))
            journey.Title = request.Title;

        if (request.Description is not null)
            journey.Description = request.Description;

        if (request.Category.HasValue)
            journey.Category = request.Category.Value;

        if (request.Status.HasValue)
        {
            journey.Status = request.Status.Value;
            if (request.Status == JourneyStatus.Completed && journey.CompletedAt is null)
                journey.CompletedAt = request.CompletedAt ?? DateTime.UtcNow;
        }

        if (request.TargetCompletedAt.HasValue)
            journey.TargetCompletedAt = request.TargetCompletedAt;

        if (request.CoverImageUrl is not null)
            journey.CoverImageUrl = request.CoverImageUrl;

        journey.UpdatedAt = DateTime.UtcNow;

        await journeyRepository.UpdateJourneyAsync(journey, cancellationToken);

        var updated = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);
        return Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(updated!, actor.UserId));
    }
}
