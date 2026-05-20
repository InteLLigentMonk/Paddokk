using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetCarJourneys;

public record GetCarJourneysQuery(
    string Username,
    string CarSlug,
    int Page = 1,
    int PageSize = 5
) : IQuery<Result<IEnumerable<JourneyDto>>>;
