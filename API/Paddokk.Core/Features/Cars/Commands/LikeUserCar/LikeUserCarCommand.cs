using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.LikeUserCar;

public record LikeUserCarCommand(int CarId) : ICommand<Result>;
