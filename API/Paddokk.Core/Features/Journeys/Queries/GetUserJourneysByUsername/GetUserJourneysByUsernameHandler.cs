using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneysByUsername;

public sealed class GetUserJourneysByUsernameHandler(
    IJourneyRepository journeyRepository,
    IUserRepository userRepository,
    IActorResolver actor)
    : IRequestHandler<GetUserJourneysByUsernameQuery, Result<IEnumerable<JourneyDto>>>
{
    public async Task<Result<IEnumerable<JourneyDto>>> Handle(
        GetUserJourneysByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        var owner = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (owner is null)
            return Result<IEnumerable<JourneyDto>>.Failure(Error.NotFound($"User '{request.Username}' not found"));

        var currentUserId = actor.IsAuthenticated ? actor.UserId : null;

        var journeys = await journeyRepository.GetUserJourneysByUsernameAsync(request.Username, currentUserId, cancellationToken);

        return Result<IEnumerable<JourneyDto>>.Success(
            journeys.Select(j => JourneyMapping.ToJourneyDto(j, currentUserId)));
    }
}
