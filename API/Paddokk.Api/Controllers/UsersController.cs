using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Features.Users.Commands.UpdateUser;
using Paddokk.Core.Features.Users.Queries.GetUserByEmail;
using Paddokk.Core.Features.Users.Queries.GetUserById;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize]
public class UsersController(ISender sender) : ApiControllerBase
{
    [HttpGet("me")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get current authenticated user's profile")]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(User.GetUserId()), ct);
        return OkOrError(result);
    }

    [HttpPut("me")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update current authenticated user's profile")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return OkOrError(result);
    }

    [HttpGet("{userId}")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get user profile by ID")]
    public async Task<ActionResult<UserDto>> GetUserById(string userId, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId), ct);
        return OkOrError(result);
    }

    [HttpGet("email/{email}")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get user profile by email")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByEmailQuery(email), ct);
        return OkOrError(result);
    }
}
