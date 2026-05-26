using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Commands.ChangeUsername;

public record ChangeUsernameCommand(string Username) : ICommand<Result<UserDto>>;
