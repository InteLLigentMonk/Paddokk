using FluentValidation;

namespace Paddokk.Core.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).When(x => x.FirstName is not null)
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(50).When(x => x.LastName is not null)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).When(x => x.DisplayName is not null)
            .WithMessage("Display name cannot exceed 100 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(500).When(x => x.Bio is not null)
            .WithMessage("Bio cannot exceed 500 characters");
    }
}
