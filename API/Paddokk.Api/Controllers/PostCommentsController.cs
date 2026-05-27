using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Comments.Commands.CreateComment;
using Paddokk.Core.Features.Comments.Commands.DeleteComment;
using Paddokk.Core.Features.Comments.Commands.UpdateComment;
using Paddokk.Core.Features.Comments.Queries.GetCommentById;
using Paddokk.Core.Features.Comments.Queries.GetPostComments;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/posts/{postId}/comments")]
public class PostCommentsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    public async Task<ActionResult<PagedResult<PostCommentDto>>> GetPostComments(
        int postId,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationDefaults.DefaultPageSize)
    {
        var result = await sender.Send(new GetPostCommentsQuery(postId, page, pageSize), ct);
        return OkOrError(result);
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("writes")]
    public async Task<ActionResult<PostCommentDto>> CreateComment(
        int postId, [FromBody] CreateCommentCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { PostId = postId }, ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return CreatedAtAction(nameof(GetComment),
            new { postId, commentId = result.Value!.Id }, result.Value);
    }

    [HttpGet("{commentId}")]
    [EnableRateLimiting("reads")]
    public async Task<ActionResult<PostCommentDto>> GetComment(
        int postId, int commentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCommentByIdQuery(commentId), ct);

        if (result.IsSuccess && result.Value!.JourneyPostId != postId)
            return NotFound();

        return OkOrError(result);
    }

    [HttpPut("{commentId}")]
    [Authorize]
    [EnableRateLimiting("writes")]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(
        int postId, int commentId, [FromBody] UpdateCommentCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { CommentId = commentId }, ct);
        return OkOrError(result);
    }

    [HttpDelete("{commentId}")]
    [Authorize]
    [EnableRateLimiting("writes")]
    public async Task<IActionResult> DeleteComment(int postId, int commentId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCommentCommand(commentId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }
}
