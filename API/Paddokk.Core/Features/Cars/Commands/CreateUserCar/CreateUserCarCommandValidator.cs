using FluentValidation;

namespace Paddokk.Core.Features.Cars.Commands.CreateUserCar;

public sealed class CreateUserCarCommandValidator : AbstractValidator<CreateUserCarCommand>
{
    public CreateUserCarCommandValidator()
    {
        When(x => !x.IsCustomBuild, () =>
        {
            RuleFor(x => x.CarMakeId)
                .NotNull().WithMessage("Car make is required")
                .GreaterThan(0).WithMessage("Car make is required");

            RuleFor(x => x.CarModelId)
                .NotNull().WithMessage("Car model is required")
                .GreaterThan(0).WithMessage("Car model is required");

            RuleFor(x => x.Year)
                .NotNull().WithMessage("Year is required")
                .GreaterThanOrEqualTo(1900).WithMessage("Year must be 1900 or later")
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Year cannot be in the future");
        });

        When(x => x.IsCustomBuild, () =>
        {
            RuleFor(x => x.CustomBuildName)
                .NotEmpty().WithMessage("Custom build name is required")
                .MaximumLength(200).WithMessage("Custom build name cannot exceed 200 characters");
        });

        RuleFor(x => x.Nickname)
            .MaximumLength(100).When(x => x.Nickname is not null)
            .WithMessage("Nickname cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .MaximumLength(50).When(x => x.Color is not null)
            .WithMessage("Color cannot exceed 50 characters");
    }
}
