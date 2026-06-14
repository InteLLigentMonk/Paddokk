using Microsoft.Extensions.Options;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Resend;

namespace Paddokk.Core.Services;

/// <summary>
/// Sends the "your data export is ready" email through Resend. Wraps the Resend <see cref="IResend"/>
/// client so the rest of the app depends only on <see cref="IExportEmailSender"/>.
/// </summary>
public sealed class ResendExportEmailSender(IResend resend, IOptions<EmailOptions> options)
    : IExportEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendExportReadyAsync(string toEmail, string downloadUrl, DateTime expiresAt, CancellationToken ct)
    {
        var expiresHuman = ExportEmailContent.FormatExpiry(expiresAt);

        var message = new EmailMessage
        {
            From = _options.FromAddress,
            Subject = ExportEmailContent.Subject,
            HtmlBody = ExportEmailContent.BuildHtmlBody(downloadUrl, expiresHuman)
        };
        message.To.Add(toEmail);

        await resend.EmailSendAsync(message, ct);
    }
}

/// <summary>Shared subject/body for the "export ready" email, reused by the dev logging sender.</summary>
internal static class ExportEmailContent
{
    public const string Subject = "Your Paddokk data export is ready";

    public static string FormatExpiry(DateTime expiresAt) =>
        expiresAt.ToUniversalTime().ToString("MMMM d, yyyy 'at' HH:mm 'UTC'");

    public static string BuildHtmlBody(string downloadUrl, string expiresHuman) =>
        $"""
        <p>Your Paddokk data export is ready.</p>
        <p><a href="{downloadUrl}">Download your data</a></p>
        <p>This link expires on <strong>{expiresHuman}</strong>. After that the file is permanently deleted
        and you'll need to request a new export.</p>
        <p>If you didn't request this export, you can safely ignore this email.</p>
        """;
}
