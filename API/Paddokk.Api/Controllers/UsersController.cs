using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Get current authenticated user's profile
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Update current authenticated user's profile
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await _userService.UpdateUserAsync(userId, request, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Get user profile by ID (public)
    /// </summary>
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUserById(string userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Get user profile by username (public)
    /// </summary>
    [HttpGet("email/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByEmailAsync(email, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(user);
    }
}
