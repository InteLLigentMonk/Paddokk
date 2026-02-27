using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPostById;

public record GetJourneyPostByIdQuery(int PostId) : IQuery<Result<JourneyPostDto>>;
