using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.CarImages.Commands.DeleteCarImage;

public record DeleteCarImageCommand(int CarId, int ImageId) : ICommand<Result>;
