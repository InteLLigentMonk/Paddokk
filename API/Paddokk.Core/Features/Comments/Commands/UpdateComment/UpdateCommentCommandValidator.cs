using FluentValidation;

namespace Paddokk.Core.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
