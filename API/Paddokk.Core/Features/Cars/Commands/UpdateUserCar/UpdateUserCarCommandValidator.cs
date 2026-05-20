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

        RuleFor(x => x.Region)
            .MaximumLength(64).When(x => x.Region is not null)
            .WithMessage("Region cannot exceed 64 characters");

        RuleFor(x => x.Engine)
            .MaximumLength(128).When(x => x.Engine is not null)
            .WithMessage("Engine cannot exceed 128 characters");

        RuleFor(x => x.OdometerKm)
            .GreaterThanOrEqualTo(0).When(x => x.OdometerKm.HasValue)
            .WithMessage("Odometer cannot be negative");

        RuleFor(x => x.OwnerNote)
            .MaximumLength(2000).When(x => x.OwnerNote is not null)
            .WithMessage("Owner note cannot exceed 2000 characters");
    }
}
