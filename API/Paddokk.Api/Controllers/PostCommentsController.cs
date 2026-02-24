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

    public PostCommentsController(ICommentService commentService)
    {
        _commentService = commentService;
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
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(await _commentService.GetPostCommentsAsync(postId, cancellationToken, page, pageSize, currentUserId));
    }

    /// <summary>
    /// Add comment to journey post
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostCommentDto>> CreateComment(int postId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _commentService.CreateCommentAsync(User.GetUserId(), postId, request, cancellationToken);
        return CreatedAtAction(nameof(GetComment), new { postId, commentId = comment.Id }, comment);
    }

    /// <summary>
    /// Get specific comment
    /// </summary>
    [HttpGet("{commentId}")]
    public async Task<ActionResult<PostCommentDto>> GetComment(int postId, int commentId, CancellationToken cancellationToken)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var comment = await _commentService.GetCommentByIdAsync(commentId, cancellationToken, currentUserId);

        if (comment is null || comment.JourneyPostId != postId)
            return NotFound();

        return Ok(comment);
    }

    /// <summary>
    /// Update comment (owner only)
    /// </summary>
    [HttpPut("{commentId}")]
    [Authorize]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(int postId, int commentId, [FromBody] UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _commentService.UpdateCommentAsync(User.GetUserId(), commentId, request, cancellationToken);

        if (comment is null)
            return NotFound();

        return Ok(comment);
    }

    /// <summary>
    /// Delete comment (owner or post owner)
    /// </summary>
    [HttpDelete("{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int postId, int commentId, CancellationToken cancellationToken)
    {
        var result = await _commentService.DeleteCommentAsync(User.GetUserId(), commentId, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
