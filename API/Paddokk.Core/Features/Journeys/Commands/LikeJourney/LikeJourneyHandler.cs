using MediatR;
using Paddokk.Core.Features.Journeys.Events;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.LikeJourney;

public sealed class LikeJourneyHandler(
    IJourneyRepository journeyRepository,
    IActorResolver actor,
    IPublisher publisher)
    : IRequestHandler<LikeJourneyCommand, Result>
{
    public async Task<Result> Handle(LikeJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null)
            return Result.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        if (journey.PrincipalId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot like your own journey"));

        var existing = await journeyRepository.GetLikeAsync(actor.UserId, request.JourneyId, cancellationToken);

        if (existing is not null)
            return Result.Success(); // idempotent — no event, no duplicate notification

        await journeyRepository.CreateLikeAsync(new JourneyLike
        {
            UserId = actor.UserId,
            JourneyId = request.JourneyId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await publisher.Publish(
            new JourneyLiked(actor.UserId, request.JourneyId, journey.PrincipalId),
            cancellationToken);

        return Result.Success();
    }
}
