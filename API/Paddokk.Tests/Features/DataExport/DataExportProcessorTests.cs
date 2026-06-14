using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;
using Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;
using Paddokk.Core.Interfaces;
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

    private DataExportRequest ClaimPending()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.ClaimNextPendingAsync(Arg.Any<CancellationToken>()).Returns(request);
        return request;
    }

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
        var request = ClaimPending();
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>()).Returns(EmptyDoc());
        _blobStore.SaveAndCreateDownloadUrlAsync("user-1", request.Id, Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new DataExportBlobResult("https://blob/data.json", "https://blob/data.json?sastoken"));

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
        await _sender.Received(1).Send(
            Arg.Is<MarkExportCompleteCommand>(c =>
                c.RequestId == request.Id &&
                c.BlobUri == "https://blob/data.json" &&
                c.DownloadUrl == "https://blob/data.json?sastoken"),
            Arg.Any<CancellationToken>());
        await _sender.DidNotReceive().Send(Arg.Any<MarkExportFailedCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessNextAsync_AssemblyThrows_MarksFailedWithoutDeletingBlob()
    {
        var request = ClaimPending();
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>())
            .Returns<DataExportDocument>(_ => throw new InvalidOperationException("boom"));

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
        await _sender.Received(1).Send(
            Arg.Is<MarkExportFailedCommand>(c => c.RequestId == request.Id), Arg.Any<CancellationToken>());
        // Nothing was uploaded, so there is no orphan blob to delete.
        await _blobStore.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _sender.DidNotReceive().Send(Arg.Any<MarkExportCompleteCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessNextAsync_CompleteFailsAfterUpload_DeletesOrphanBlobAndMarksFailed()
    {
        var request = ClaimPending();
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>()).Returns(EmptyDoc());
        _blobStore.SaveAndCreateDownloadUrlAsync("user-1", request.Id, Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new DataExportBlobResult("https://blob/data.json", "https://blob/data.json?sastoken"));
        _sender.Send(Arg.Any<MarkExportCompleteCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("db down"));

        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
        // The already-written blob must be removed so it is not orphaned (the Failed row carries no BlobUrl).
        await _blobStore.Received(1).DeleteAsync("https://blob/data.json", Arg.Any<CancellationToken>());
        await _sender.Received(1).Send(
            Arg.Is<MarkExportFailedCommand>(c => c.RequestId == request.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessNextAsync_MarkFailedAlsoThrows_DoesNotPropagate()
    {
        var request = ClaimPending();
        _assembler.BuildAsync("user-1", Arg.Any<CancellationToken>())
            .Returns<DataExportDocument>(_ => throw new InvalidOperationException("boom"));
        _sender.Send(Arg.Any<MarkExportFailedCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("db down too"));

        // Must not throw — a failure to mark failed cannot be allowed to wedge the worker loop.
        var processed = await _processor.ProcessNextAsync(CancellationToken.None);

        processed.Should().BeTrue();
    }
}
