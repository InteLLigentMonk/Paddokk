using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(string UserId) : IQuery<Result<UserDto>>;
