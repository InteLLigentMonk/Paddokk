using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByUsername;

public record GetUserByUsernameQuery(string Username) : IQuery<Result<UserDto>>;
