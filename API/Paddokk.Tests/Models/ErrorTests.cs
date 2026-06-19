using FluentAssertions;
using Paddokk.Core.Models;

namespace Paddokk.Tests.Models;

public class ErrorTests
{
    [Fact]
    public void NotFound_DefaultsToNotFoundCode()
    {
        var error = Error.NotFound("missing");

        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be(ErrorCodes.NotFound);
    }

    [Fact]
    public void Conflict_DefaultsToConflictCode()
    {
        Error.Conflict("nope").Code.Should().Be(ErrorCodes.Conflict);
    }

    [Fact]
    public void Validation_DefaultsToValidationFailedCode()
    {
        Error.Validation("bad").Code.Should().Be(ErrorCodes.ValidationFailed);
    }

    [Fact]
    public void Unauthorized_DefaultsToForbiddenCode()
    {
        Error.Unauthorized("no").Code.Should().Be(ErrorCodes.Forbidden);
    }

    [Fact]
    public void Internal_DefaultsToInternalCode()
    {
        Error.Internal("boom").Code.Should().Be(ErrorCodes.Internal);
    }

    [Fact]
    public void Conflict_WithExplicitCode_UsesOverride()
    {
        var error = Error.Conflict("own journey", ErrorCodes.SubscribeToOwnSubject);

        error.Type.Should().Be(ErrorType.Conflict);
        error.Code.Should().Be(ErrorCodes.SubscribeToOwnSubject);
    }

    [Fact]
    public void None_HasEmptyCode()
    {
        Error.None.Code.Should().BeEmpty();
    }

    [Fact]
    public void IsKnown_RecognizesStableCodes_AndRejectsFluentValidationNames()
    {
        ErrorCodes.IsKnown(ErrorCodes.UploadTooLarge).Should().BeTrue();
        ErrorCodes.IsKnown("NotEmptyValidator").Should().BeFalse();
        ErrorCodes.IsKnown(null).Should().BeFalse();
    }
}
