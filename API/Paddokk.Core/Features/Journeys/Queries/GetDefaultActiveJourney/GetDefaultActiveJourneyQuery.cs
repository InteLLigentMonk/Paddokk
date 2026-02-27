using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetDefaultActiveJourney;

public record GetDefaultActiveJourneyQuery : IQuery<Result<JourneyDto>>;
