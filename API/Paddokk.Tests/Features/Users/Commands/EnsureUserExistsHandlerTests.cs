using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Common;
using Paddokk.Core.Features.Users.Commands.EnsureUserExists;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Users.Commands;

public class EnsureUserExistsHandlerTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly UsernameGenerator _generator = new();
    private readonly EnsureUserExistsHandler _handler;

    public EnsureUserExistsHandlerTests()
    {
        _handler = new EnsureUserExistsHandler(_repo, _generator);
        _repo.UsernameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
    }

    [Fact]
    public async Task Handle_UserAlreadyExists_DoesNotCreate()
    {
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new ApplicationUser { Id = "user-1" });

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", "Tobias Vinther", null, null),
            CancellationToken.None);

        await _repo.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NewUserWithGivenAndFamily_CreatesWithSplitNames()
    {
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "tobias@example.com", null, "Tobias", "Vinther"),
            CancellationToken.None);

        captured.Value.Should().NotBeNull();
        captured.Value!.FirstName.Should().Be("Tobias");
        captured.Value.LastName.Should().Be("Vinther");
        captured.Value.DisplayName.Should().Be("Tobias Vinther");
        captured.Value.Username.Should().Be("tobias.vinther");
        captured.Value.Email.Should().Be("tobias@example.com");
        captured.Value.Id.Should().Be("user-1");
    }

    [Fact]
    public async Task Handle_NewUserWithFullNameOnly_SplitsOnFirstSpace()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", "Tobias Vinther", null, null),
            CancellationToken.None);

        captured.Value!.FirstName.Should().Be("Tobias");
        captured.Value.LastName.Should().Be("Vinther");
    }

    [Fact]
    public async Task Handle_NewUserWithFullNameMultipleSpaces_KeepsRestInLastName()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", "Tobias Anders Vinther", null, null),
            CancellationToken.None);

        captured.Value!.FirstName.Should().Be("Tobias");
        captured.Value.LastName.Should().Be("Anders Vinther");
    }

    [Fact]
    public async Task Handle_NewUserWithSingleNameOnly_NoLastName()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", "Tobias", null, null),
            CancellationToken.None);

        captured.Value!.FirstName.Should().Be("Tobias");
        captured.Value.LastName.Should().BeNull();
        captured.Value.Username.Should().Be("tobias");
        captured.Value.DisplayName.Should().Be("Tobias");
    }

    [Fact]
    public async Task Handle_UsernameCollision_AddsSuffix()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        _repo.UsernameExistsAsync("tobias.vinther", Arg.Any<CancellationToken>()).Returns(true);
        _repo.UsernameExistsAsync("tobias.vinther.1", Arg.Any<CancellationToken>()).Returns(false);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", null, "Tobias", "Vinther"),
            CancellationToken.None);

        captured.Value!.Username.Should().Be("tobias.vinther.1");
    }

    [Fact]
    public async Task Handle_NoNameAnywhere_FallsBackToEmailPrefix()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "tobias@example.com", null, null, null),
            CancellationToken.None);

        captured.Value!.FirstName.Should().Be("tobias");
        captured.Value.LastName.Should().BeNull();
        captured.Value.Username.Should().Be("tobias");
        captured.Value.DisplayName.Should().Be("tobias");
    }

    [Fact]
    public async Task Handle_PrefersGivenNameOverFullName()
    {
        _repo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var captured = CaptureCreate();

        await _handler.Handle(
            new EnsureUserExistsCommand("user-1", "a@b.com", "Should Be Ignored", "Tobias", "Vinther"),
            CancellationToken.None);

        captured.Value!.FirstName.Should().Be("Tobias");
        captured.Value.LastName.Should().Be("Vinther");
    }

    private CapturedUser CaptureCreate()
    {
        var captured = new CapturedUser();
        _repo.When(r => r.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>()))
            .Do(ci => captured.Value = ci.ArgAt<ApplicationUser>(0));
        return captured;
    }

    private sealed class CapturedUser
    {
        public ApplicationUser? Value { get; set; }
    }
}
