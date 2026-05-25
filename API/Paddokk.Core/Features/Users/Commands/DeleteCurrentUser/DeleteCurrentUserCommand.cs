using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Users.Commands.DeleteCurrentUser;

public record DeleteCurrentUserCommand : ICommand<Result>;
