using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.SetDefaultActiveJourney;

public record SetDefaultActiveJourneyCommand(int JourneyId) : ICommand<Result>;
