using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(int CommentId) : ICommand<Result>;
