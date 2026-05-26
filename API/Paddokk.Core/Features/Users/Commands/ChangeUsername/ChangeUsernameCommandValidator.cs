using FluentValidation;

namespace Paddokk.Core.Features.Users.Commands.ChangeUsername;

public sealed class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(2).WithMessage("Username must be at least 2 characters")
            .MaximumLength(30).WithMessage("Username cannot exceed 30 characters")
            .Matches("^[a-z0-9._-]+$")
                .WithMessage("Username may only contain lowercase letters, digits, dot, underscore and hyphen");
    }
}
