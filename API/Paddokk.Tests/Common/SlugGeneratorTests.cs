using FluentAssertions;
using Paddokk.Core.Common;

namespace Paddokk.Tests.Common;

public class SlugGeneratorTests
{
    private readonly SlugGenerator _generator = new();

    [Theory]
    [InlineData("My Build", "my-build")]
    [InlineData("Hello World", "hello-world")]
    [InlineData("MyBuild", "mybuild")]
    [InlineData("MY BUILD", "my-build")]
    [InlineData("  Trimmed  ", "trimmed")]
    [InlineData("  Multiple   Spaces  ", "multiple-spaces")]
    public void Generate_BasicTitles_ProducesKebabCase(string input, string expected)
    {
        _generator.Generate(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("Åsa's Volvo", "asas-volvo")]
    [InlineData("Café Racer", "cafe-racer")]
    [InlineData("Müller's Garage", "mullers-garage")]
    [InlineData("São Paulo Build", "sao-paulo-build")]
    public void Generate_TransliteratesDiacritics(string input, string expected)
    {
        _generator.Generate(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("Project: Midnight (1JZ Swap)", "project-midnight-1jz-swap")]
    [InlineData("240 GL", "240-gl")]
    [InlineData("Track Day!!!", "track-day")]
    [InlineData("Build #42", "build-42")]
    [InlineData("R34 / GTR", "r34-gtr")]
    public void Generate_StripsSpecialCharsKeepsAlphanumerics(string input, string expected)
    {
        _generator.Generate(input).Should().Be(expected);
    }

    [Fact]
    public void Generate_LongInput_TruncatesToMaxLength()
    {
        var input = string.Concat(Enumerable.Repeat("very-long-build-name-segment ", 10));
        var result = _generator.Generate(input);
        result.Length.Should().BeLessThanOrEqualTo(80);
    }

    [Fact]
    public void Generate_TruncationDoesNotEndWithDash()
    {
        // Ensure we don't truncate mid-separator leaving dangling "-"
        var input = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-extra-bit";
        var result = _generator.Generate(input);
        result.Should().NotEndWith("-");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!!!")]
    [InlineData("???---")]
    public void Generate_NoValidChars_Throws(string input)
    {
        var act = () => _generator.Generate(input);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task EnsureUniqueAsync_NoConflict_ReturnsCandidate()
    {
        var seen = new HashSet<string>();
        Func<string, string, CancellationToken, Task<bool>> exists =
            (principalId, slug, _) => Task.FromResult(seen.Contains($"{principalId}:{slug}"));

        var result = await _generator.EnsureUniqueAsync("my-build", "user-1", exists, CancellationToken.None);

        result.Should().Be("my-build");
    }

    [Fact]
    public async Task EnsureUniqueAsync_OneConflict_SuffixesWithDash2()
    {
        var taken = new HashSet<string> { "user-1:my-build" };
        Func<string, string, CancellationToken, Task<bool>> exists =
            (principalId, slug, _) => Task.FromResult(taken.Contains($"{principalId}:{slug}"));

        var result = await _generator.EnsureUniqueAsync("my-build", "user-1", exists, CancellationToken.None);

        result.Should().Be("my-build-2");
    }

    [Fact]
    public async Task EnsureUniqueAsync_MultipleConflicts_ContinuesSuffixing()
    {
        var taken = new HashSet<string>
        {
            "user-1:my-build",
            "user-1:my-build-2",
            "user-1:my-build-3"
        };
        Func<string, string, CancellationToken, Task<bool>> exists =
            (principalId, slug, _) => Task.FromResult(taken.Contains($"{principalId}:{slug}"));

        var result = await _generator.EnsureUniqueAsync("my-build", "user-1", exists, CancellationToken.None);

        result.Should().Be("my-build-4");
    }

    [Fact]
    public async Task EnsureUniqueAsync_PerUserScope_DifferentUsersDoNotConflict()
    {
        var taken = new HashSet<string> { "user-1:my-build" };
        Func<string, string, CancellationToken, Task<bool>> exists =
            (principalId, slug, _) => Task.FromResult(taken.Contains($"{principalId}:{slug}"));

        var result = await _generator.EnsureUniqueAsync("my-build", "user-2", exists, CancellationToken.None);

        result.Should().Be("my-build");
    }

    [Fact]
    public async Task EnsureUniqueAsync_ExceedsMaxAttempts_Throws()
    {
        Func<string, string, CancellationToken, Task<bool>> alwaysExists =
            (_, _, _) => Task.FromResult(true);

        var act = async () => await _generator.EnsureUniqueAsync(
            "my-build", "user-1", alwaysExists, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
