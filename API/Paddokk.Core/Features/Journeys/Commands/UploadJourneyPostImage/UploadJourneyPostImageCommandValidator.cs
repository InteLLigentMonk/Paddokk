using FluentValidation;
using Paddokk.Core.Common.ImageUpload;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyPostImage;

public sealed class UploadJourneyPostImageCommandValidator : AbstractValidator<UploadJourneyPostImageCommand>
{
    public UploadJourneyPostImageCommandValidator(IImageUploadValidator imageValidator)
    {
        RuleFor(x => x.File).Custom((file, ctx) =>
        {
            var result = imageValidator.Validate(file);
            if (!result.IsValid)
                ctx.AddFailure("File", result.Reason!);
        });
    }
}
