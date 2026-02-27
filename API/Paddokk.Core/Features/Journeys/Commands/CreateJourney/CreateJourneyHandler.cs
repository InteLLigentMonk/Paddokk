using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourney;

public sealed class CreateJourneyHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<CreateJourneyCommand, Result<JourneyDto>>
{
    public async Task<Result<JourneyDto>> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
    {
        var user = await journeyRepository.GetUserAsync(actor.UserId, cancellationToken);
        if (user is null)
            return Result<JourneyDto>.Failure(Error.NotFound("User not found"));

        var currentCount = await journeyRepository.GetUserJourneyCountAsync(actor.UserId, cancellationToken);
        var maxJourneys = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        if (currentCount >= maxJourneys)
            return Result<JourneyDto>.Failure(Error.Conflict("Journey limit reached for current subscription tier"));

        var journey = new Journey
        {
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Status = JourneyStatus.Active,
            UserId = actor.UserId,
            UserCarId = request.UserCarId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await journeyRepository.CreateJourneyAsync(journey, cancellationToken);

        if (request.SetAsDefaultActive)
            await journeyRepository.UpdateUserDefaultJourneyAsync(actor.UserId, journey.Id, cancellationToken);

        var created = await journeyRepository.GetJourneyByIdAsync(journey.Id, cancellationToken);
        return created is null
            ? Result<JourneyDto>.Failure(Error.Internal("Failed to retrieve created journey"))
            : Result<JourneyDto>.Success(JourneyMapping.ToJourneyDto(created, actor.UserId));
    }
}
