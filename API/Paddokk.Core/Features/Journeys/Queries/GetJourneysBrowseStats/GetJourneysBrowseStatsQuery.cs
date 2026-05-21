using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneysBrowseStats;

public record GetJourneysBrowseStatsQuery(JourneySearchRequest Search) : IQuery<Result<GetJourneysBrowseStatsResponse>>;
