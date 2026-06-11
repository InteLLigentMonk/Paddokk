using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.DataExport.Queries.GetDataExportStatus;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport.Queries;

public class GetDataExportStatusHandlerTests
{
    private readonly IDataExportRepository _repo = Substitute.For<IDataExportRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetDataExportStatusHandler _handler;

    public GetDataExportStatusHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new GetDataExportStatusHandler(_repo, _actor);
    }

    [Fact]
    public async Task Handle_OwnRequest_ReturnsDto()
    {
        var request = new DataExportRequest { UserId = "user-1", Status = DataExportStatus.Pending };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await _handler.Handle(new GetDataExportStatusQuery(request.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(request.Id);
    }

    [Fact]
    public async Task Handle_OtherUsersRequest_ReturnsNotFound()
    {
        var request = new DataExportRequest { UserId = "someone-else", Status = DataExportStatus.Ready };
        _repo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await _handler.Handle(new GetDataExportStatusQuery(request.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_UnknownId_ReturnsNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DataExportRequest?)null);

        var result = await _handler.Handle(new GetDataExportStatusQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
