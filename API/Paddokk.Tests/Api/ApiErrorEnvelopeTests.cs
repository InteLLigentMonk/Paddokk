using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Paddokk.Api.Controllers;
using Paddokk.Api.Middleware;
using Paddokk.Api.OpenApi;
using Paddokk.Core.Common.ImageUpload;
using Paddokk.Core.Features.CarImages.Commands.UploadCarImage;
using Paddokk.Core.Models;
using Paddokk.Tests.Common;

namespace Paddokk.Tests.Api;

public class ApiErrorEnvelopeTests
{
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    private sealed class TestController : ApiControllerBase
    {
        public ActionResult Expose(Error error) => FromError(error);
    }

    // --- ApiControllerBase.FromError ---

    [Fact]
    public void FromError_Conflict_ReturnsEnvelopeWithCodeMessageAndStatus()
    {
        var result = new TestController().Expose(
            Error.Conflict("own journey", ErrorCodes.SubscribeToOwnSubject));

        var obj = result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var body = obj.Value.Should().BeOfType<ApiErrorResponse>().Subject;
        body.Code.Should().Be(ErrorCodes.SubscribeToOwnSubject);
        body.Message.Should().Be("own journey");
        body.Status.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public void FromError_NotFound_Maps404WithNotFoundCode()
    {
        var obj = new TestController().Expose(Error.NotFound("gone"))
            .Should().BeOfType<ObjectResult>().Subject;

        obj.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        ((ApiErrorResponse)obj.Value!).Code.Should().Be(ErrorCodes.NotFound);
    }

    [Fact]
    public void FromError_Unauthorized_Maps403WithBody()
    {
        var obj = new TestController().Expose(Error.Unauthorized("denied"))
            .Should().BeOfType<ObjectResult>().Subject;

        obj.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        ((ApiErrorResponse)obj.Value!).Code.Should().Be(ErrorCodes.Forbidden);
    }

    [Fact]
    public void FromError_Internal_Maps500()
    {
        var obj = new TestController().Expose(Error.Internal("boom"))
            .Should().BeOfType<ObjectResult>().Subject;

        obj.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        ((ApiErrorResponse)obj.Value!).Code.Should().Be(ErrorCodes.Internal);
    }

    // --- GlobalExceptionMiddleware ---

    private static async Task<(int status, ApiErrorResponse body)> RunMiddlewareAsync(Exception ex)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionMiddleware(
            _ => throw ex,
            Substitute.For<ILogger<GlobalExceptionMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var raw = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var dto = JsonSerializer.Deserialize<ApiErrorResponse>(raw, Web)!;
        return (context.Response.StatusCode, dto);
    }

    [Fact]
    public async Task ValidationException_WithUploadCode_SurfacesTopLevelCodeAndPerFailure()
    {
        var failure = new ValidationFailure("File", "too big") { ErrorCode = ErrorCodes.UploadTooLarge };

        var (status, body) = await RunMiddlewareAsync(new ValidationException([failure]));

        status.Should().Be(StatusCodes.Status400BadRequest);
        body.Code.Should().Be(ErrorCodes.UploadTooLarge);
        body.Errors.Should().ContainSingle();
        body.Errors![0].Field.Should().Be("File");
        body.Errors[0].Code.Should().Be(ErrorCodes.UploadTooLarge);
        body.Errors[0].Message.Should().Be("too big");
    }

    [Fact]
    public async Task ValidationException_WithUnknownFluentCode_NormalizesToValidationFailed()
    {
        var failure = new ValidationFailure("Name", "required") { ErrorCode = "NotEmptyValidator" };

        var (status, body) = await RunMiddlewareAsync(new ValidationException([failure]));

        status.Should().Be(StatusCodes.Status400BadRequest);
        body.Code.Should().Be(ErrorCodes.ValidationFailed);
        body.Errors![0].Code.Should().Be(ErrorCodes.ValidationFailed);
    }

    [Fact]
    public async Task GenericException_Returns500WithInternalCode_AndDoesNotLeakMessage()
    {
        var (status, body) = await RunMiddlewareAsync(new Exception("secret stack detail"));

        status.Should().Be(StatusCodes.Status500InternalServerError);
        body.Code.Should().Be(ErrorCodes.Internal);
        body.Message.Should().NotContain("secret");
    }

    [Fact]
    public async Task KeyNotFoundException_Returns404WithNotFoundCode()
    {
        var (status, body) = await RunMiddlewareAsync(new KeyNotFoundException("nope"));

        status.Should().Be(StatusCodes.Status404NotFound);
        body.Code.Should().Be(ErrorCodes.NotFound);
    }

    // --- Upload command validator wires the stable code through to the failure ---

    [Fact]
    public async Task UploadCarImageCommandValidator_OversizeFile_FailsWithUploadTooLargeCode()
    {
        var validator = new UploadCarImageCommandValidator(new ImageUploadValidator());
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/jpeg",
            lengthOverride: ImageUploadValidator.MaxFileSizeBytes + 1);

        var result = await validator.ValidateAsync(new UploadCarImageCommand(1, file, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ErrorCode.Should().Be(ErrorCodes.UploadTooLarge);
    }
}
