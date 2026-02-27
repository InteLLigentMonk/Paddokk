using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

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
            await journeyRepository.UpdateUserDefaultJourneyAsync(actor.UserId, null, cancellationToken);

        await journeyRepository.DeleteJourneyAsync(request.JourneyId, cancellationToken);
        return Result.Success();
    }
}
