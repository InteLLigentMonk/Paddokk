using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.SetDefaultActiveJourney;

public sealed class SetDefaultActiveJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<SetDefaultActiveJourneyCommand, Result>
{
    public async Task<Result> Handle(SetDefaultActiveJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null || journey.UserId != actor.UserId)
            return Result.Failure(Error.NotFound("Journey not found or you don't own it"));

        await journeyRepository.UpdateUserDefaultJourneyAsync(actor.UserId, request.JourneyId, cancellationToken);
        return Result.Success();
    }
}
