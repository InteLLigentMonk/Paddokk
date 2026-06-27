using MediatR;
using Microsoft.Extensions.Options;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.DataExport;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport.Commands.RequestDataExport;

public sealed class RequestDataExportHandler(
    IDataExportRepository repository,
    IActorResolver actor,
    IOptions<DataExportOptions> options)
    : IRequestHandler<RequestDataExportCommand, Result<DataExportRequestDto>>
{
    private readonly DataExportOptions _options = options.Value;

    public async Task<Result<DataExportRequestDto>> Handle(RequestDataExportCommand command, CancellationToken ct)
    {
        var userId = actor.UserId;

        // Already building one — hand back the in-flight request instead of queueing a duplicate.
        var outstanding = await repository.GetOutstandingForUserAsync(userId, ct);
        if (outstanding is not null)
            return Result<DataExportRequestDto>.Success(DataExportMapping.ToDto(outstanding));

        // Cooldown: block a fresh export until CooldownHours after the last completed/failed one.
        var lastCompleted = await repository.GetMostRecentCompletedForUserAsync(userId, ct);
        if (lastCompleted?.CompletedAt is { } completedAt &&
            completedAt > DateTime.UtcNow.AddHours(-_options.CooldownHours))
        {
            return Result<DataExportRequestDto>.Failure(Error.Conflict(
                $"A data export was requested recently. Try again after {_options.CooldownHours} hours.",
                ErrorCodes.ExportCooldown));
        }

        var request = new DataExportRequest
        {
            UserId = userId,
            Status = DataExportStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await repository.AddAsync(request, ct);

        return Result<DataExportRequestDto>.Success(DataExportMapping.ToDto(request));
    }
}
