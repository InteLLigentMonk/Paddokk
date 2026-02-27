using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetFeaturedJourneys;

public record GetFeaturedJourneysQuery : IQuery<IEnumerable<JourneyDto>>;
