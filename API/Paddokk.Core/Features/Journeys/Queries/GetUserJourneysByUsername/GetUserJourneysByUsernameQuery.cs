using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetUserJourneysByUsername;

public record GetUserJourneysByUsernameQuery(string Username) : IQuery<Result<IEnumerable<JourneyDto>>>;
