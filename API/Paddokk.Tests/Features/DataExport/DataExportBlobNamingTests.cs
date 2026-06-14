using FluentAssertions;
using Paddokk.Core.Features.DataExport;

namespace Paddokk.Tests.Features.DataExport;

public class DataExportBlobNamingTests
{
    [Fact]
    public void BuildBlobName_SafeUserId_ReturnsUserScopedJsonPath()
    {
        var requestId = Guid.Parse("0b2c0000-0000-0000-0000-000000000000");

        var name = DataExportBlobNaming.BuildBlobName("wCDuEfSYM8ch", requestId);

        name.Should().Be($"wCDuEfSYM8ch/{requestId}.json");
    }

    [Theory]
    [InlineData("../etc/passwd")]
    [InlineData("user/with/slashes")]
    [InlineData("user with spaces")]
    [InlineData("")]
    public void BuildBlobName_UnsafeUserId_Throws(string userId)
    {
        var act = () => DataExportBlobNaming.BuildBlobName(userId, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }
}
