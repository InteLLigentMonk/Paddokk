using FluentValidation;
using Paddokk.Core.Common.ImageUpload;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyCoverImage;

public sealed class UploadJourneyCoverImageCommandValidator : AbstractValidator<UploadJourneyCoverImageCommand>
{
    public UploadJourneyCoverImageCommandValidator(IImageUploadValidator imageValidator)
    {
        RuleFor(x => x.File).Custom((file, ctx) =>
        {
            var result = imageValidator.Validate(file);
            if (!result.IsValid)
                ctx.AddFailure("File", result.Reason!);
        });
    }
}
