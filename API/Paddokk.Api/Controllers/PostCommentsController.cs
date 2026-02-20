using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
public class PostCommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IJourneyService _journeyService;
    private readonly ILogger<PostCommentsController> _logger;

    public PostCommentsController(
        ICommentService commentService,
        IJourneyService journeyService,
        ILogger<PostCommentsController> logger)
    {
        _commentService = commentService;
        _journeyService = journeyService;
        _logger = logger;
    }

    /// <summary>
    /// Get comments for a journey post
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CommentsPagedResponse>> GetPostComments(
        int postId,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            // Validate page parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
            var comments = await _commentService.GetPostCommentsAsync(postId, page, pageSize, currentUserId, cancellationToken);

            return Ok(comments);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add comment to journey post
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostCommentDto>> CreateComment(int postId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            var comment = await _commentService.CreateCommentAsync(userId, postId, request, cancellationToken);

            return CreatedAtAction(nameof(GetComment), new { postId, commentId = comment.Id }, comment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get specific comment
    /// </summary>
    [HttpGet("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> GetComment(int postId, int commentId, CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (string?)null;
        var comment = await _commentService.GetCommentByIdAsync(commentId, currentUserId, cancellationToken);

        if (comment == null || comment.JourneyPostId != postId)
            return NotFound(new { message = "Comment not found" });

        return Ok(comment);
    }

    /// <summary>
    /// Update comment (owner only)
    /// </summary>
    [HttpPut("{commentId}")]
    [Authorize]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(int postId, int commentId, [FromBody] UpdateCommentRequest request, CancellationToken cancellationToken)
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
    [Authorize]
    public async Task<IActionResult> DeleteComment(int postId, int commentId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _commentService.DeleteCommentAsync(userId, commentId, cancellationToken);

        if (!result)
            return NotFound(new { message = "Comment not found or you don't have permission to delete it" });

        return NoContent();
    }
}
