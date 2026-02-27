using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.SubscribeToJourney;

public sealed class SubscribeToJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<SubscribeToJourneyCommand, Result>
{
    public async Task<Result> Handle(SubscribeToJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null)
            return Result.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        if (journey.UserId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot subscribe to your own journey"));

        var existing = await journeyRepository.GetSubscriptionAsync(actor.UserId, request.JourneyId, cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await journeyRepository.UpdateSubscriptionAsync(existing, cancellationToken);
            }

            return Result.Success();
        }

        await journeyRepository.CreateSubscriptionAsync(new JourneySubscription
        {
            UserId = actor.UserId,
            JourneyId = request.JourneyId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }, cancellationToken);

        return Result.Success();
    }
}
