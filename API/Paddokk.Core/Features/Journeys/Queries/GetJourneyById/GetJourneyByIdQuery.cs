using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyById;

public record GetJourneyByIdQuery(int JourneyId) : IQuery<Result<JourneyDto>>;
