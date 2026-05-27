using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.Comments.Commands.DeleteComment;
using Paddokk.Core.Features.Comments.Commands.UpdateComment;
using Paddokk.Core.Features.Comments.Queries.GetCommentById;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/comments")]
[Authorize]
public class CommentsController(ISender sender) : ApiControllerBase
{
    [HttpGet("{commentId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get specific comment by ID")]
    public async Task<ActionResult<PostCommentDto>> GetComment(int commentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCommentByIdQuery(commentId), ct);
        return OkOrError(result);
    }

    [HttpPut("{commentId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update comment (owner only)")]
    public async Task<ActionResult<PostCommentDto>> UpdateComment(
        int commentId, [FromBody] UpdateCommentCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { CommentId = commentId }, ct);
        return OkOrError(result);
    }

    [HttpDelete("{commentId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete comment (owner or post owner)")]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCommentCommand(commentId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }

    [HttpPost("{commentId}/report")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Report comment for moderation")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult ReportComment(int commentId, [FromBody] string reason)
    {
        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Moderation coming soon",
            detail: "Comment reporting is not yet available. Please contact support@paddokk.com if you need to flag this comment.");
    }
}
