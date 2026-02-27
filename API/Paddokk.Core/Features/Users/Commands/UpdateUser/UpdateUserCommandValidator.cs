using FluentValidation;

namespace Paddokk.Core.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(100).When(x => x.DisplayName is not null)
            .WithMessage("Display name cannot exceed 100 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(500).When(x => x.Bio is not null)
            .WithMessage("Bio cannot exceed 500 characters");
    }
}
