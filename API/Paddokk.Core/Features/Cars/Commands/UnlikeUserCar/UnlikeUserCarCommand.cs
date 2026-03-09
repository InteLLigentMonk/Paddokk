using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.UnlikeUserCar;

public record UnlikeUserCarCommand(int CarId) : ICommand<Result>;
