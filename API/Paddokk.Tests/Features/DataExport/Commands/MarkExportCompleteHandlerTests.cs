using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport.Commands;

public class MarkExportCompleteHandlerTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IExportEmailSender _email = Substitute.For<IExportEmailSender>();
    private readonly MarkExportCompleteHandler _handler;

    public MarkExportCompleteHandlerTests()
    {
        _handler = new MarkExportCompleteHandler(_repo, _userRepo, _email, NullLogger<MarkExportCompleteHandler>.Instance);
    }

    [Fact]
    public async Task Handle_BuildingRequest_TransitionsToReadyAndSendsEmailOnce()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _userRepo.GetByIdAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new ApplicationUser { Id = "user-1", Email = "user@example.com" });
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var result = await _handler.Handle(
            new MarkExportCompleteCommand(request.Id, "https://blob/sas", expiresAt), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        request.Status.Should().Be(DataExportStatus.Ready);
        request.BlobUrl.Should().Be("https://blob/sas");
        request.ExpiresAt.Should().Be(expiresAt);
        request.CompletedAt.Should().NotBeNull();
        await _repo.Received(1).UpdateAsync(request, Arg.Any<CancellationToken>());
        await _email.Received(1).SendExportReadyAsync("user@example.com", "https://blob/sas", expiresAt, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyReady_IsNoOpAndDoesNotResendEmail()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Ready };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await _handler.Handle(
            new MarkExportCompleteCommand(request.Id, "https://blob/sas", DateTime.UtcNow.AddDays(7)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
        await _email.DidNotReceive().SendExportReadyAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownId_ReturnsNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);

        var result = await _handler.Handle(
            new MarkExportCompleteCommand(Guid.NewGuid(), "https://blob/sas", DateTime.UtcNow.AddDays(7)),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
