using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.UnsubscribeFromJourney;

public sealed class UnsubscribeFromJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<UnsubscribeFromJourneyCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeFromJourneyCommand request, CancellationToken cancellationToken)
    {
        var subscription = await journeyRepository.GetSubscriptionAsync(actor.UserId, request.JourneyId, cancellationToken);

        if (subscription is null)
            return Result.Success(); // idempotent

        subscription.IsActive = false;
        await journeyRepository.UpdateSubscriptionAsync(subscription, cancellationToken);
        return Result.Success();
    }
}
