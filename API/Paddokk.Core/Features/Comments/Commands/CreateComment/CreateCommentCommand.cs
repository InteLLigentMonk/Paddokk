using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand(int PostId, string Content, int? ParentCommentId = null)
    : ICommand<Result<PostCommentDto>>;
