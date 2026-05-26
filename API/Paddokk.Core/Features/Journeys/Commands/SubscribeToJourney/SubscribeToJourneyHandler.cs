using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.SubscribeToJourney;

public sealed class SubscribeToJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<SubscribeToJourneyCommand, Result>
{
    public Task<Result> Handle(SubscribeToJourneyCommand request, CancellationToken cancellationToken) =>
        Subscriptions.SubscribeAsync(
            new SubjectLookup<Journey>(
                Label: "Journey",
                LoadAsync: ct => journeyRepository.GetJourneyByIdAsync(request.JourneyId, ct),
                PrincipalIdOf: journey => journey.PrincipalId),
            new SubscriptionOps<JourneySubscription>(
                FindAsync: ct => journeyRepository.GetSubscriptionAsync(actor.UserId, request.JourneyId, ct),
                CreateAsync: (sub, ct) => journeyRepository.CreateSubscriptionAsync(sub, ct),
                UpdateAsync: (sub, ct) => journeyRepository.UpdateSubscriptionAsync(sub, ct)),
            newRelation: () => new JourneySubscription
            {
                UserId = actor.UserId,
                JourneyId = request.JourneyId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            actorUserId: actor.UserId,
            cancellationToken);
}
