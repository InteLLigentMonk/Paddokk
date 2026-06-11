using MediatR;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;

/// <summary>
/// Internal command issued by the background worker once the export blob is written. Idempotent:
/// re-issuing it for an already-Ready request is a no-op and does not resend the email.
/// <paramref name="DownloadUrl"/> is the time-limited SAS link emailed to the user.
/// </summary>
public sealed record MarkExportCompleteCommand(Guid RequestId, string DownloadUrl, DateTime ExpiresAt)
    : IRequest<Result>;
