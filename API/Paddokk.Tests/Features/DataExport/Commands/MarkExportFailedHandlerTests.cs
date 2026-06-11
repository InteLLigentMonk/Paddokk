using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport.Commands;

public class MarkExportFailedHandlerTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly MarkExportFailedHandler _handler;

    public MarkExportFailedHandlerTests()
    {
        _handler = new MarkExportFailedHandler(_repo);
    }

    [Fact]
    public async Task Handle_BuildingRequest_TransitionsToFailed()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await _handler.Handle(new MarkExportFailedCommand(request.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        request.Status.Should().Be(DataExportStatus.Failed);
        request.CompletedAt.Should().NotBeNull();
        await _repo.Received(1).UpdateAsync(request, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyReady_IsNoOp()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Ready };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await _handler.Handle(new MarkExportFailedCommand(request.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        request.Status.Should().Be(DataExportStatus.Ready);
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }
}
