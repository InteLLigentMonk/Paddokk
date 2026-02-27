namespace Paddokk.Core.Features.Journeys.Queries.CanCreateJourney;

public record CanCreateJourneyResponse(
    bool CanCreate,
    int CurrentCount,
    string MaxJourneys,
    string SubscriptionTier
);
