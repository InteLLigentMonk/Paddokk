using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport;

public class DataExportCleanupServiceTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly IDataExportBlobStore _blobStore = Substitute.For<IDataExportBlobStore>();
    private readonly DataExportCleanupService _service;

    public DataExportCleanupServiceTests()
    {
        var options = Options.Create(new DataExportOptions { StuckBuildingThresholdMinutes = 60 });
        _service = new DataExportCleanupService(_repo, _blobStore, options, NullLogger<DataExportCleanupService>.Instance);

        // Default both reads to empty so each test only seeds the slice it exercises.
        _repo.GetExpiredReadyAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns([]);
        _repo.GetStuckBuildingAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns([]);
    }

    private static DataExportRequest Expired(string blobUrl) => new()
    {
        UserId = "user-1",
        Status = DataExportStatus.Ready,
        BlobUrl = blobUrl,
        ExpiresAt = DateTime.UtcNow.AddHours(-1)
    };

    [Fact]
    public async Task RunAsync_ExpiredReadyRequests_MarksExpiredAndDeletesBlobs()
    {
        var expired = Expired("https://blob/data.json");
        _repo.GetExpiredReadyAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(new[] { expired });

        var count = await _service.RunAsync(CancellationToken.None);

        count.Should().Be(1);
        expired.Status.Should().Be(DataExportStatus.Expired);
        await _blobStore.Received(1).DeleteAsync("https://blob/data.json", Arg.Any<CancellationToken>());
        await _repo.Received(1).UpdateAsync(expired, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_NoWork_DoesNothing()
    {
        var count = await _service.RunAsync(CancellationToken.None);

        count.Should().Be(0);
        await _blobStore.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_OneBlobDeleteFails_StillExpiresRemainingRows()
    {
        var failing = Expired("https://blob/bad.json");
        var ok = Expired("https://blob/good.json");
        _repo.GetExpiredReadyAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(new[] { failing, ok });
        _blobStore.DeleteAsync("https://blob/bad.json", Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("azure hiccup"));

        await _service.RunAsync(CancellationToken.None);

        // The failing row is left for the next sweep; the healthy row is still expired.
        failing.Status.Should().Be(DataExportStatus.Ready);
        ok.Status.Should().Be(DataExportStatus.Expired);
        await _repo.Received(1).UpdateAsync(ok, Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateAsync(failing, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_StuckBuildingRows_ReclaimedToFailed()
    {
        var stuck = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Building };
        _repo.GetStuckBuildingAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(new[] { stuck });

        await _service.RunAsync(CancellationToken.None);

        stuck.Status.Should().Be(DataExportStatus.Failed);
        stuck.CompletedAt.Should().NotBeNull();
        await _repo.Received(1).UpdateAsync(stuck, Arg.Any<CancellationToken>());
    }
}
