using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.UnsubscribeFromUserCar;

public record UnsubscribeFromUserCarCommand(int CarId) : ICommand<Result>;
