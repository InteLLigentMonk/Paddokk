using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Features.DataExport.Commands.RequestDataExport;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport.Commands;

public class RequestDataExportHandlerTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly RequestDataExportHandler _handler;

    public RequestDataExportHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        var options = Options.Create(new DataExportOptions { CooldownHours = 24 });
        _handler = new RequestDataExportHandler(_repo, _actor, options);
    }

    [Fact]
    public async Task Handle_NoExistingRequests_CreatesPendingAndReturnsId()
    {
        _repo.GetOutstandingForUserAsync("user-1", Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);
        _repo.GetMostRecentCompletedForUserAsync("user-1", Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);

        var result = await _handler.Handle(new RequestDataExportCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(DataExportStatus.Pending);
        await _repo.Received(1).AddAsync(
            Arg.Is<DataExportRequest>(r => r.UserId == "user-1" && r.Status == DataExportStatus.Pending),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InFlightRequestExists_ReturnsExistingWithoutCreating()
    {
        var existing = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.GetOutstandingForUserAsync("user-1", Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new RequestDataExportCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(existing.Id);
        await _repo.DidNotReceive().AddAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RecentCompletedWithinCooldown_ReturnsConflictWithoutCreating()
    {
        _repo.GetOutstandingForUserAsync("user-1", Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);
        _repo.GetMostRecentCompletedForUserAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new DataExportRequest
            {
                UserId = "user-1",
                Status = DataExportStatus.Ready,
                CompletedAt = DateTime.UtcNow.AddHours(-1)
            });

        var result = await _handler.Handle(new RequestDataExportCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        await _repo.DidNotReceive().AddAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CompletedBeyondCooldown_CreatesNewPending()
    {
        _repo.GetOutstandingForUserAsync("user-1", Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);
        _repo.GetMostRecentCompletedForUserAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new DataExportRequest
            {
                UserId = "user-1",
                Status = DataExportStatus.Failed,
                CompletedAt = DateTime.UtcNow.AddHours(-25)
            });

        var result = await _handler.Handle(new RequestDataExportCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(DataExportStatus.Pending);
        await _repo.Received(1).AddAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }
}
