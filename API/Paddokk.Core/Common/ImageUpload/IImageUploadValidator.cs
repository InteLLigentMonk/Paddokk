using Microsoft.AspNetCore.Http;

namespace Paddokk.Core.Common.ImageUpload;

public interface IImageUploadValidator
{
    ImageValidationResult Validate(IFormFile? file);
}
