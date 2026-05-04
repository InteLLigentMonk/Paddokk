using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourney;

public sealed class DeleteJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<DeleteJourneyCommand, Result>
{
    public async Task<Result> Handle(DeleteJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null || journey.UserId != actor.UserId)
            return Result.Failure(Error.NotFound("Journey not found or you don't own it"));

        var user = await journeyRepository.GetUserAsync(actor.UserId, cancellationToken);
        if (user?.DefaultActiveJourneyId == request.JourneyId)
        {
            var allJourneys = await journeyRepository.GetUserJourneysAsync(actor.UserId, cancellationToken);
            var nextDefault = allJourneys
                .FirstOrDefault(j => j.Id != request.JourneyId && j.Status == JourneyStatus.Active);
            await journeyRepository.UpdateUserDefaultJourneyAsync(actor.UserId, nextDefault?.Id, cancellationToken);
        }

        await journeyRepository.DeleteJourneyAsync(request.JourneyId, cancellationToken);
        return Result.Success();
    }
}
