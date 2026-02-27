using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.LikeJourney;

public record LikeJourneyCommand(int JourneyId) : ICommand<Result>;
