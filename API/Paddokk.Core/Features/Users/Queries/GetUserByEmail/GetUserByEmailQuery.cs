using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Core.Features.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IQuery<Result<UserDto>>;
