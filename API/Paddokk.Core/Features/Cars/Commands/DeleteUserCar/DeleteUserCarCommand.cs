using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Commands.DeleteUserCar;

public record DeleteUserCarCommand(int CarId) : ICommand<Result>;
