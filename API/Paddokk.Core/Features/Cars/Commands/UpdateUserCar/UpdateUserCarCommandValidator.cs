using FluentValidation;

namespace Paddokk.Core.Features.Cars.Commands.UpdateUserCar;

public sealed class UpdateUserCarCommandValidator : AbstractValidator<UpdateUserCarCommand>
{
    public UpdateUserCarCommandValidator()
    {
        RuleFor(x => x.Nickname)
            .MaximumLength(100).When(x => x.Nickname is not null)
            .WithMessage("Nickname cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .MaximumLength(50).When(x => x.Color is not null)
            .WithMessage("Color cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
