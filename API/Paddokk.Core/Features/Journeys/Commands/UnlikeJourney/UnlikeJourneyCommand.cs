using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.UnlikeJourney;

public record UnlikeJourneyCommand(int JourneyId) : ICommand<Result>;
