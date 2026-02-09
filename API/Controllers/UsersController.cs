using API.Extensions;
using API.Models.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            _logger.LogInformation("Getting current user...");

            var userId = User.GetUserId();
            _logger.LogInformation("Retrieved user ID: {UserId}", userId);

            var user = await _userService.GetUserByIdAsync(userId);
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
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var user = await _userService.UpdateUserAsync(userId, request);

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
    public async Task<ActionResult<UserDto>> GetUserById(string userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);

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
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);

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
