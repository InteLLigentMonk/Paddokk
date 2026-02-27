using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Queries.CanCreateJourney;

public sealed class CanCreateJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<CanCreateJourneyQuery, CanCreateJourneyResponse>
{
    public async Task<CanCreateJourneyResponse> Handle(CanCreateJourneyQuery request, CancellationToken cancellationToken)
    {
        var user = await journeyRepository.GetUserAsync(actor.UserId, cancellationToken);
        var currentCount = await journeyRepository.GetUserJourneyCountAsync(actor.UserId, cancellationToken);

        var maxJourneys = (user?.SubscriptionTier ?? SubscriptionTier.Free) switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        return new CanCreateJourneyResponse(
            CanCreate: currentCount < maxJourneys,
            CurrentCount: currentCount,
            MaxJourneys: maxJourneys == int.MaxValue ? "Unlimited" : maxJourneys.ToString(),
            SubscriptionTier: user?.SubscriptionTier.ToString() ?? "Free"
        );
    }
}
