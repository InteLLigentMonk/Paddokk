using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.UnsubscribeFromJourney;

public sealed class UnsubscribeFromJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<UnsubscribeFromJourneyCommand, Result>
{
    public Task<Result> Handle(UnsubscribeFromJourneyCommand request, CancellationToken cancellationToken) =>
        Subscriptions.UnsubscribeAsync(
            new ToggleOps<JourneySubscription>(
                FindAsync: ct => journeyRepository.GetSubscriptionAsync(actor.UserId, request.JourneyId, ct),
                UpdateAsync: (sub, ct) => journeyRepository.UpdateSubscriptionAsync(sub, ct)),
            cancellationToken);
}
