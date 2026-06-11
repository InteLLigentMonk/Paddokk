using MediatR;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportFailed;

/// <summary>
/// Internal command issued by the worker when assembly or upload fails. Transitions a non-terminal
/// request to Failed so the user becomes eligible to request a fresh export after the cooldown.
/// Idempotent against already-terminal requests.
/// </summary>
public sealed record MarkExportFailedCommand(Guid RequestId) : IRequest<Result>;
