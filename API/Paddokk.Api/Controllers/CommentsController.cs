using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// Get specific comment by ID
    /// </summary>
    [HttpGet("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> GetComment(int commentId, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var comment = await _commentService.GetCommentByIdAsync(commentId, currentUserId, cancellationToken);

        if (comment == null)
            return NotFound(new { message = "Comment not found" });

        return Ok(comment);
    }

    /// <summary>
    /// Update comment (owner only)
    /// </summary>
    [HttpPut("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(int commentId, [FromBody] UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var comment = await _commentService.UpdateCommentAsync(userId, commentId, request, cancellationToken);

        if (comment == null)
            return NotFound(new { message = "Comment not found or you don't have permission to edit it" });

        return Ok(comment);
    }

    /// <summary>
    /// Delete comment (owner or post owner)
    /// </summary>
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _commentService.DeleteCommentAsync(userId, commentId, cancellationToken);

        if (!result)
            return NotFound(new { message = "Comment not found or you don't have permission to delete it" });

        return NoContent();
    }

    /// <summary>
    /// Report comment for moderation
    /// </summary>
    [HttpPost("{commentId}/report")]
    public async Task<IActionResult> ReportComment(int commentId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _commentService.ReportCommentAsync(userId, commentId, reason, cancellationToken);

        if (!result)
            return BadRequest(new { message = "Failed to report comment" });

        return Ok(new { message = "Comment reported for moderation" });
    }
}
