using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Features.Cars.Queries.GetUserCarBySlug;
using Paddokk.Core.Features.Cars.Queries.GetUserCarsByUsername;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyBySlug;
using Paddokk.Core.Features.Journeys.Queries.GetUserJourneysByUsername;
using Paddokk.Core.Features.Users.Commands.UpdateUser;
using Paddokk.Core.Features.Users.Queries.GetUserByEmail;
using Paddokk.Core.Features.Users.Queries.GetUserById;
using Paddokk.Core.Features.Users.Queries.GetUserByUsername;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.DTOs.Journey;
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

    [HttpGet("by-username/{username}")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get user profile by username")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByUsernameQuery(username), ct);
        return OkOrError(result);
    }

    [HttpGet("by-username/{username}/cars")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a user's cars (filtered by visibility)")]
    public async Task<ActionResult<IEnumerable<UserCarDto>>> GetUserCarsByUsername(string username, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserCarsByUsernameQuery(username), ct);
        return OkOrError(result);
    }

    [HttpGet("by-username/{username}/cars/{slug}")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a user's car by slug")]
    public async Task<ActionResult<UserCarDto>> GetUserCarBySlug(string username, string slug, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserCarBySlugQuery(username, slug), ct);
        return OkOrError(result);
    }

    [HttpGet("by-username/{username}/journeys")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a user's journeys (filtered by visibility)")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetUserJourneysByUsername(string username, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserJourneysByUsernameQuery(username), ct);
        return OkOrError(result);
    }

    [HttpGet("by-username/{username}/journeys/{slug}")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a user's journey by slug")]
    public async Task<ActionResult<JourneyDto>> GetJourneyBySlug(string username, string slug, CancellationToken ct)
    {
        var result = await sender.Send(new GetJourneyBySlugQuery(username, slug), ct);
        return OkOrError(result);
    }
}
