using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.SearchJourneys;

public record SearchJourneysQuery(JourneySearchRequest Search) : IQuery<Result<PagedJourneysResponse>>;
