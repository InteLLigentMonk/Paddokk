using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.SubscribeToUserCar;

public record SubscribeToUserCarCommand(int CarId) : ICommand<Result>;
