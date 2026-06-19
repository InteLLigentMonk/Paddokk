using FluentValidation;
using FluentValidation.Results;
using Paddokk.Core.Common.ImageUpload;

namespace Paddokk.Core.Features.CarImages.Commands.UploadCarImage;

public sealed class UploadCarImageCommandValidator : AbstractValidator<UploadCarImageCommand>
{
    public UploadCarImageCommandValidator(IImageUploadValidator imageValidator)
    {
        RuleFor(x => x.File).Custom((file, ctx) =>
        {
            var result = imageValidator.Validate(file);
            if (!result.IsValid)
                ctx.AddFailure(new ValidationFailure("File", result.Reason!) { ErrorCode = result.Code });
        });
    }
}
