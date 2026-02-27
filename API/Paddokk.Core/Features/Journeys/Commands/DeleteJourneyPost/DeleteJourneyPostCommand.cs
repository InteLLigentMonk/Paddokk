using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPost;

public record DeleteJourneyPostCommand(int PostId) : ICommand<Result>;
