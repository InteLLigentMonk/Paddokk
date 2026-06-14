using FluentAssertions;
using Paddokk.Core.Common;

namespace Paddokk.Tests.Common;

public class BlobUrlParserTests
{
    [Fact]
    public void Parse_SimpleBlobUrl_ReturnsContainerAndBlobName()
    {
        var result = BlobUrlParser.Parse("https://acct.blob.core.windows.net/car-images/abc.webp");

        result.Should().NotBeNull();
        result!.Value.Container.Should().Be("car-images");
        result.Value.BlobName.Should().Be("abc.webp");
    }

    [Fact]
    public void Parse_NestedBlobName_KeepsSubPath()
    {
        var result = BlobUrlParser.Parse("https://acct.blob.core.windows.net/data-exports/user-1/0b2c.json");

        result!.Value.Container.Should().Be("data-exports");
        result.Value.BlobName.Should().Be("user-1/0b2c.json");
    }

    [Fact]
    public void Parse_UrlWithSasQuery_IgnoresQueryString()
    {
        var result = BlobUrlParser.Parse("https://acct.blob.core.windows.net/data-exports/user-1/0b2c.json?sig=abc&se=2026");

        result!.Value.Container.Should().Be("data-exports");
        result.Value.BlobName.Should().Be("user-1/0b2c.json");
    }

    [Fact]
    public void Parse_NoBlobSegment_ReturnsNull()
    {
        var result = BlobUrlParser.Parse("https://acct.blob.core.windows.net/only-container");

        result.Should().BeNull();
    }
}
