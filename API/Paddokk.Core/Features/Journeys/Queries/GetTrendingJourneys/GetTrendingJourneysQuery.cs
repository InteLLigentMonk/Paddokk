using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetTrendingJourneys;

public record GetTrendingJourneysQuery : IQuery<IEnumerable<JourneyDto>>;
