using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.SearchJourneys;

public record SearchJourneysQuery(JourneySearchRequest Search) : IQuery<IEnumerable<JourneyDto>>;
