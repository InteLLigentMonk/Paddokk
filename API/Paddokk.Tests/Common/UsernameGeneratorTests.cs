using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;

namespace Paddokk.Tests.Common;

public class UsernameGeneratorTests
{
    private readonly UsernameGenerator _generator = new();

    [Theory]
    [InlineData("Tobias", "Vinther", "tobias.vinther")]
    [InlineData("tobias", "vinther", "tobias.vinther")]
    [InlineData("TOBIAS", "VINTHER", "tobias.vinther")]
    [InlineData("Tobias", null, "tobias")]
    [InlineData("Tobias", "", "tobias")]
    [InlineData("Tobias", "  ", "tobias")]
    [InlineData("  Tobias  ", "  Vinther  ", "tobias.vinther")]
    public void Generate_BasicNames_ProducesLowercaseDotSeparated(string first, string? last, string expected)
    {
        var result = _generator.Generate(first, last);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Åsa", "Öberg", "asa.oberg")]
    [InlineData("Émile", "Müller", "emile.muller")]
    [InlineData("André", "Citroën", "andre.citroen")]
    public void Generate_TransliteratesDiacritics(string first, string last, string expected)
    {
        var result = _generator.Generate(first, last);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Tobias!", "Vinther@", "tobias.vinther")]
    [InlineData("To bias", "Vin-ther", "tobias.vinther")]
    [InlineData("O'Brien", "McLeod", "obrien.mcleod")]
    public void Generate_StripsNonAlphanumeric(string first, string last, string expected)
    {
        var result = _generator.Generate(first, last);
        result.Should().Be(expected);
    }

    [Fact]
    public void Generate_LongNames_TruncatesToMaxLength()
    {
        var result = _generator.Generate("Veryverylongfirstname", "Andsuperlonglastnametoo");
        result.Length.Should().BeLessThanOrEqualTo(30);
    }

    [Fact]
    public void Generate_EmptyOrWhitespaceFirstName_Throws()
    {
        var act = () => _generator.Generate("   ", "Vinther");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Generate_FirstNameOnlySpecialChars_Throws()
    {
        var act = () => _generator.Generate("!!!", null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task EnsureUniqueAsync_NoConflict_ReturnsCandidate()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.UsernameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _generator.EnsureUniqueAsync("tobias.vinther", repo, CancellationToken.None);

        result.Should().Be("tobias.vinther");
    }

    [Fact]
    public async Task EnsureUniqueAsync_OneConflict_ReturnsSuffixedCandidate()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.UsernameExistsAsync("tobias.vinther", Arg.Any<CancellationToken>()).Returns(true);
        repo.UsernameExistsAsync("tobias.vinther.1", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _generator.EnsureUniqueAsync("tobias.vinther", repo, CancellationToken.None);

        result.Should().Be("tobias.vinther.1");
    }

    [Fact]
    public async Task EnsureUniqueAsync_ReservedWord_ReturnsSuffixedCandidate()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.UsernameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _generator.EnsureUniqueAsync("admin", repo, CancellationToken.None);

        result.Should().Be("admin.1");
    }

    [Fact]
    public async Task EnsureUniqueAsync_MultipleConflicts_KeepsTrying()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.UsernameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.ArgAt<string>(0) != "tobias.vinther.5");

        var result = await _generator.EnsureUniqueAsync("tobias.vinther", repo, CancellationToken.None);

        result.Should().Be("tobias.vinther.5");
    }

    [Fact]
    public async Task EnsureUniqueAsync_ExceedsMaxAttempts_Throws()
    {
        var repo = Substitute.For<IUserRepository>();
        repo.UsernameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await _generator.EnsureUniqueAsync("tobias.vinther", repo, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
