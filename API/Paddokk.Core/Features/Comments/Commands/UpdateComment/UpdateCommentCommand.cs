using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(int CommentId, string Content)
    : ICommand<Result<PostCommentDto>>;
