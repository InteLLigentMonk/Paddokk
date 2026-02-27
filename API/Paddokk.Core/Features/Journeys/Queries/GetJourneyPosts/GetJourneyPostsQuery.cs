using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;

public record GetJourneyPostsQuery(int JourneyId, int Skip = 0, int Take = 20) : IQuery<Result<IEnumerable<JourneyPostDto>>>;
