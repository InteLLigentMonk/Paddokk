using MediatR;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.DataExport;

namespace Paddokk.Core.Features.DataExport.Commands.RequestDataExport;

/// <summary>
/// Requests a GDPR data export for the current actor. The owning user is taken from the
/// authenticated actor, never from the request body, so a user can only export their own data.
/// </summary>
public sealed record RequestDataExportCommand : IRequest<Result<DataExportRequestDto>>;
