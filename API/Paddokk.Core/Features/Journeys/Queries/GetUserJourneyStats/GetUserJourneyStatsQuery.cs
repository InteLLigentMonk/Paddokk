using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneyStats;

public record GetUserJourneyStatsQuery : IQuery<JourneyStatsDto>;
