using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneyStats;

public sealed class GetUserJourneyStatsHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<GetUserJourneyStatsQuery, JourneyStatsDto>
{
    public async Task<JourneyStatsDto> Handle(GetUserJourneyStatsQuery request, CancellationToken cancellationToken)
    {
        var journeys = await journeyRepository.GetUserJourneysWithStatsAsync(actor.UserId, cancellationToken);

        return new JourneyStatsDto
        {
            TotalJourneys = journeys.Count,
            ActiveJourneys = journeys.Count(j => j.Status == JourneyStatus.Active),
            CompletedJourneys = journeys.Count(j => j.Status == JourneyStatus.Completed),
            TotalPosts = journeys.Sum(j => j.Posts.Count),
            TotalSubscribers = journeys.Sum(j => j.Subscriptions.Count(s => s.IsActive)),
            TotalLikes = journeys.Sum(j => j.Likes.Count)
        };
    }
}
