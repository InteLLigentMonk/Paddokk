using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Users.Commands.DeleteCurrentUser;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Users.Commands;

public class DeleteCurrentUserHandlerTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly DeleteCurrentUserHandler _handler;

    public DeleteCurrentUserHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new DeleteCurrentUserHandler(_repo, _actor);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);

        var result = await _handler.Handle(new DeleteCurrentUserCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_SoftDeletesAndAnonymizes()
    {
        var user = NewUser();
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        var before = DateTime.UtcNow;
        var result = await _handler.Handle(new DeleteCurrentUserCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.IsDeleted.Should().BeTrue();
        user.DeletedAt.Should().NotBeNull();
        user.DeletedAt!.Value.Should().BeOnOrAfter(before);
        user.DisplayName.Should().Be("Deleted User");
        user.Email.Should().BeNull();
        user.Bio.Should().BeNull();
        user.AvatarUrl.Should().BeNull();
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeNull();
        user.Username.Should().StartWith("deleted-");
        user.Username.Length.Should().Be("deleted-".Length + 8);
    }

    [Fact]
    public async Task Handle_ReservesOriginalUsername()
    {
        var user = NewUser();
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        ReservedUsername? captured = null;
        await _repo.ReserveUsernameAsync(
            Arg.Do<ReservedUsername>(r => captured = r),
            Arg.Any<CancellationToken>());

        var before = DateTime.UtcNow;
        await _handler.Handle(new DeleteCurrentUserCommand(), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Slug.Should().Be("tobias");
        captured.OriginalUserId.Should().Be("user-1");
        captured.ReservedAt.Should().BeOnOrAfter(before);
        captured.ReleaseAfter.Should().BeOnOrAfter(before.AddDays(179));
    }

    [Fact]
    public async Task Handle_PersistsUpdatedUser()
    {
        var user = NewUser();
        _repo.GetByIdAsync("user-1", Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new DeleteCurrentUserCommand(), CancellationToken.None);

        await _repo.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    private static ApplicationUser NewUser() => new()
    {
        Id = "user-1",
        Username = "tobias",
        FirstName = "Tobias",
        LastName = "Vinther",
        DisplayName = "Tobias Vinther",
        Email = "tobias@example.com",
        Bio = "Hello",
        AvatarUrl = "https://example.com/avatar.png"
    };
}
