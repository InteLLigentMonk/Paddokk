using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;
using Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.DataExport;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport;

public class DataExportProcessorTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly IDataExportAssembler _assembler = Substitute.For<IDataExportAssembler>();
    private readonly IDataExportBlobStore _blobStore = Substitute.For<IDataExportBlobStore>();
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly DataExportProcessor _processor;

    public DataExportProcessorTests()
    {
        var options = Options.Create(new DataExportOptions { ExportTtlDays = 7 });
        _processor = new DataExportProcessor(
            _repo, _assembler, _blobStore, _sender, options, NullLogger<DataExportProcessor>.Instance);
    }

    private static DataExportDocument EmptyDoc() => new(
        DateTime.UtcNow,
        new DataExportProfile("u", "u", "U", "U", null, null, null, null, default),
        [], [], [], [], [], []);

    [Fact]
    public async Task ProcessNextAsync_NoPending_ReturnsFalse()
    {
        _repo.ClaimNextPendingAsync(Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeFalse();
        await _assembler.DidNotReceive().BuildAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessNextAsync_PendingRequest_AssemblesUploadsAndMarksComplete()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.ClaimNextPendingAsync(Arg.Any<CancellationToken>()).Returns(request);
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>()).Returns(EmptyDoc());
        _blobStore.SaveAndCreateDownloadUrlAsync("user-1", request.Id, Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns("https://blob/sas-link");

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
        await _blobStore.Received(1).SaveAndCreateDownloadUrlAsync(
            "user-1", request.Id, Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _sender.Received(1).Send(
            Arg.Is<MarkExportCompleteCommand>(c => c.RequestId == request.Id && c.DownloadUrl == "https://blob/sas-link"),
            Arg.Any<CancellationToken>());
        await _sender.DidNotReceive().Send(Arg.Any<MarkExportFailedCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessNextAsync_AssemblyThrows_MarksFailed()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.ClaimNextPendingAsync(Arg.Any<CancellationToken>()).Returns(request);
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>())
            .Returns<DataExportDocument>(_ => throw new InvalidOperationException("boom"));

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
        await _sender.Received(1).Send(
            Arg.Is<MarkExportFailedCommand>(c => c.RequestId == request.Id), Arg.Any<CancellationToken>());
        await _sender.DidNotReceive().Send(Arg.Any<MarkExportCompleteCommand>(), Arg.Any<CancellationToken>());
    }
}
