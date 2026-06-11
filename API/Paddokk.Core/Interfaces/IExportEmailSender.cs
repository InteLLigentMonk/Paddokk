namespace Paddokk.Core.Interfaces;

/// <summary>
/// Sends the "your data export is ready" email. Abstracts the concrete email provider (Resend) so
/// handlers and the worker stay decoupled and testable.
/// </summary>
public interface IExportEmailSender
{
    Task SendExportReadyAsync(string toEmail, string downloadUrl, DateTime expiresAt, CancellationToken cancellationToken);
}
