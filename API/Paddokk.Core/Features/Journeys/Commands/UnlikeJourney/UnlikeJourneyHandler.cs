using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.UnlikeJourney;

public sealed class UnlikeJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<UnlikeJourneyCommand, Result>
{
    public async Task<Result> Handle(UnlikeJourneyCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null)
            return Result.Failure(Error.NotFound($"Journey {request.JourneyId} not found"));

        if (journey.PrincipalId == actor.UserId)
            return Result.Failure(Error.Conflict("Cannot unlike your own journey"));

        var like = await journeyRepository.GetLikeAsync(actor.UserId, request.JourneyId, cancellationToken);

        if (like is null)
            return Result.Success(); // idempotent

        await journeyRepository.DeleteLikeAsync(actor.UserId, request.JourneyId, cancellationToken);
        return Result.Success();
    }
}
