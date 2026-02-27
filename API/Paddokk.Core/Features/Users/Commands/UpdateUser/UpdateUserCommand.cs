using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    string? DisplayName,
    string? Bio,
    string? AvatarUrl
) : ICommand<Result<UserDto>>;
