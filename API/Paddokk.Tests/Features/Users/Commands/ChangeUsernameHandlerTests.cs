using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Users.Commands.ChangeUsername;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Users.Commands;

public class ChangeUsernameHandlerTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly ChangeUsernameHandler _handler;

    public ChangeUsernameHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new ChangeUsernameHandler(_repo, _actor);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new ChangeUsernameCommand("newname"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_SameAsCurrent_IsNoOpSuccess()
    {
        var user = NewUser(username: "tobias");
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new ChangeUsernameCommand("tobias"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UsernameTaken_ReturnsConflict()
    {
        var user = NewUser(username: "tobias");
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);
        _repo.UsernameExistsAsync("anna", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new ChangeUsernameCommand("anna"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_RecentlyChanged_ReturnsConflict()
    {
        var user = NewUser(username: "tobias");
        user.LastUsernameChangeAt = DateTime.UtcNow.AddDays(-5);
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new ChangeUsernameCommand("newname"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_RateLimitExpired_AllowsChange()
    {
        var user = NewUser(username: "tobias");
        user.LastUsernameChangeAt = DateTime.UtcNow.AddDays(-31);
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);
        _repo.UsernameExistsAsync("newname", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new ChangeUsernameCommand("newname"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.Username.Should().Be("newname");
    }

    [Fact]
    public async Task Handle_ReservedWord_ReturnsConflict()
    {
        var user = NewUser(username: "tobias");
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new ChangeUsernameCommand("admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_ValidChange_UpdatesUsernameAndTimestamp()
    {
        var user = NewUser(username: "tobias");
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);
        _repo.UsernameExistsAsync("newname", Arg.Any<CancellationToken>()).Returns(false);

        var before = DateTime.UtcNow;
        var result = await _handler.Handle(new ChangeUsernameCommand("newname"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.Username.Should().Be("newname");
        user.LastUsernameChangeAt.Should().NotBeNull();
        user.LastUsernameChangeAt!.Value.Should().BeOnOrAfter(before);
        await _repo.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MixedCaseInput_NormalizedToLower()
    {
        var user = NewUser(username: "tobias");
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);
        _repo.UsernameExistsAsync("newname", Arg.Any<CancellationToken>()).Returns(false);

        await _handler.Handle(new ChangeUsernameCommand("NewName"), CancellationToken.None);

        user.Username.Should().Be("newname");
    }

    private static ApplicationUser NewUser(string username) => new()
    {
        Id = "user-1",
        Username = username,
        FirstName = "Tobias",
        DisplayName = "Tobias",
        Email = "tobias@example.com"
    };
}
