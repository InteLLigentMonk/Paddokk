using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.CarImages.Commands.SetPrimaryImage;

public record SetPrimaryImageCommand(int CarId, int ImageId) : ICommand<Result>;
