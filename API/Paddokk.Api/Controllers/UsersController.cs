using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Cars.Queries.GetUserCarBySlug;
using Paddokk.Core.Features.Follows.Commands.FollowUser;
using Paddokk.Core.Features.Follows.Commands.UnfollowUser;
using Paddokk.Core.Features.Follows.Queries.GetFollowers;
using Paddokk.Core.Features.Follows.Queries.GetFollowing;
using Paddokk.Core.Features.Cars.Queries.GetUserCarsByUsername;
using Paddokk.Core.Features.Journeys.Queries.GetCarJourneys;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyBySlug;
using Paddokk.Core.Features.Journeys.Queries.GetUserJourneysByUsername;
using Paddokk.Core.Features.Users.Commands.ChangeUsername;
using Paddokk.Core.Features.Users.Commands.DeleteCurrentUser;
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

    [HttpPatch("me/username")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Change current authenticated user's username (rate-limited)")]
    public async Task<ActionResult<UserDto>> ChangeCurrentUsername([FromBody] ChangeUsernameCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return OkOrError(result);
    }

    [HttpDelete("me")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Soft-delete current authenticated user (anonymises PII, reserves username)")]
    public async Task<ActionResult> DeleteCurrentUser(CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCurrentUserCommand(), ct);
        return OkOrError(result);
    }

    [HttpPost("{id}/follow")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Follow a user")]
    public async Task<IActionResult> FollowUser(string id, CancellationToken ct)
    {
        var result = await sender.Send(new FollowUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : FromError(result.Error);
    }

    [HttpDelete("{id}/follow")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Unfollow a user")]
    public async Task<IActionResult> UnfollowUser(string id, CancellationToken ct)
    {
        var result = await sender.Send(new UnfollowUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : FromError(result.Error);
    }

    [HttpGet("{id}/followers")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a user's followers (paged, public)")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetFollowers(
        string id,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetFollowersQuery(id, page, pageSize), ct);
        return OkOrError(result);
    }

    [HttpGet("{id}/following")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get users a user follows (paged, public)")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetFollowing(
        string id,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetFollowingQuery(id, page, pageSize), ct);
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
    public async Task<ActionResult<IEnumerable<UserCarDto>>> GetUserCarsByUsername(string username, [FromQuery] int? limit, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserCarsByUsernameQuery(username, limit), ct);
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

    [HttpGet("by-username/{username}/cars/{carSlug}/journeys")]
    [AllowAnonymous]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get journeys for a specific car (filtered by visibility)")]
    public async Task<ActionResult<IEnumerable<JourneyDto>>> GetCarJourneys(
        string username,
        string carSlug,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await sender.Send(new GetCarJourneysQuery(username, carSlug, page, pageSize), ct);
        return OkOrError(result);
    }
}
