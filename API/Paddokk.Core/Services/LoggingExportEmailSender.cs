using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Services;

/// <summary>
/// Development <see cref="IExportEmailSender"/> used when "Development:LogEmailsOnly" is set. Logs the
/// export-ready email (including the download link) instead of sending it through Resend, so local
/// runs don't need a Resend API key or a verified domain.
/// </summary>
public sealed class LoggingExportEmailSender(ILogger<LoggingExportEmailSender> logger) : IExportEmailSender
{
    public Task SendExportReadyAsync(string toEmail, string downloadUrl, DateTime expiresAt, CancellationToken ct)
    {
        logger.LogInformation(
            "[DEV email] Data export ready -> {ToEmail}; download {DownloadUrl}; expires {ExpiresAt:u}",
            toEmail, downloadUrl, expiresAt);
        return Task.CompletedTask;
    }
}
