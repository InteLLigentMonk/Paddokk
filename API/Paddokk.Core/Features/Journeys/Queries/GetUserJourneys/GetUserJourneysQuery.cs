using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneys;

public record GetUserJourneysQuery : IQuery<IEnumerable<JourneyDto>>;
