using MediatR;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.DataExport.Commands.MarkExportComplete;

/// <summary>
/// Internal command issued by the background worker once the export blob is written. Idempotent:
/// re-issuing it for an already-Ready request is a no-op and does not resend the email.
/// <paramref name="DownloadUrl"/> is the time-limited SAS link emailed to the user.
/// <para>
/// Deliberately a plain <see cref="IRequest{T}"/> rather than <c>ICommand</c>: it must NOT run under
/// the <c>TransactionBehaviour</c>, because it sends an external email after persisting Ready. If the
/// email send were inside a DB transaction, an email failure would roll back the Ready transition and
/// orphan the already-written blob.
/// </para>
/// </summary>
public sealed record MarkExportCompleteCommand(Guid RequestId, string DownloadUrl, DateTime ExpiresAt)
    : IRequest<Result>;
