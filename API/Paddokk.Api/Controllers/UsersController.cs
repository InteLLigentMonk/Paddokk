using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.User;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current authenticated user's profile
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting current user...");

            var userId = User.GetUserId();
            _logger.LogInformation("Retrieved user ID: {UserId}", userId);

            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            _logger.LogInformation("Retrieved user from service: {User}", user != null ? "Found" : "Not Found");

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update current authenticated user's profile
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            var user = await _userService.UpdateUserAsync(userId, request, cancellationToken);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", User.GetUserId());
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get user profile by ID (public)
    /// </summary>
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUserById(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get user profile by username (public)
    /// </summary>
    [HttpGet("email/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email, cancellationToken);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
