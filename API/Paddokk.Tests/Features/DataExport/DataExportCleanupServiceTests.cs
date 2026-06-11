using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
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
        _service = new DataExportCleanupService(_repo, _blobStore, NullLogger<DataExportCleanupService>.Instance);
    }

    [Fact]
    public async Task RunAsync_ExpiredReadyRequests_MarksExpiredAndDeletesBlobs()
    {
        var expired = new DataExportRequest
        {
            UserId = "user-1",
            Status = DataExportStatus.Ready,
            BlobUrl = "https://blob/sas-link",
            ExpiresAt = DateTime.UtcNow.AddHours(-1)
        };
        _repo.GetExpiredReadyAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new[] { expired });

        var count = await _service.RunAsync(CancellationToken.None);

        count.Should().Be(1);
        expired.Status.Should().Be(DataExportStatus.Expired);
        await _blobStore.Received(1).DeleteAsync("https://blob/sas-link", Arg.Any<CancellationToken>());
        await _repo.Received(1).UpdateAsync(expired, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_NoExpired_DoesNothing()
    {
        _repo.GetExpiredReadyAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns([]);

        var count = await _service.RunAsync(CancellationToken.None);

        count.Should().Be(0);
        await _blobStore.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<DataExportRequest>(), Arg.Any<CancellationToken>());
    }
}
