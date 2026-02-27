using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.UnsubscribeFromJourney;

public record UnsubscribeFromJourneyCommand(int JourneyId) : ICommand<Result>;
