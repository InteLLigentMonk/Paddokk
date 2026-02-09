using API.Extensions;
using API.Models.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get specific comment by ID
    /// </summary>
    [HttpGet("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> GetComment(int commentId)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var comment = await _commentService.GetCommentByIdAsync(commentId, currentUserId);

        if (comment == null)
            return NotFound(new { message = "Comment not found" });

        return Ok(comment);
    }

    /// <summary>
    /// Update comment (owner only)
    /// </summary>
    [HttpPut("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(int commentId, [FromBody] UpdateCommentRequest request)
    {
        var userId = User.GetUserId();
        var comment = await _commentService.UpdateCommentAsync(userId, commentId, request);

        if (comment == null)
            return NotFound(new { message = "Comment not found or you don't have permission to edit it" });

        return Ok(comment);
    }

    /// <summary>
    /// Delete comment (owner or post owner)
    /// </summary>
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var userId = User.GetUserId();
        var result = await _commentService.DeleteCommentAsync(userId, commentId);

        if (!result)
            return NotFound(new { message = "Comment not found or you don't have permission to delete it" });

        return NoContent();
    }

    /// <summary>
    /// Report comment for moderation
    /// </summary>
    [HttpPost("{commentId}/report")]
    public async Task<IActionResult> ReportComment(int commentId, [FromBody] string reason)
    {
        var userId = User.GetUserId();
        var result = await _commentService.ReportCommentAsync(userId, commentId, reason);

        if (!result)
            return BadRequest(new { message = "Failed to report comment" });

        return Ok(new { message = "Comment reported for moderation" });
    }
}
