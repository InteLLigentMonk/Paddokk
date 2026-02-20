using System.Text.Encodings.Web;
using Azure;
using Azure.Communication.Email;
using Paddokk.Core.Interfaces;

namespace Paddokk.Api.Services;

public class AzureEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureEmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromDisplayName;

    public AzureEmailService(
        EmailClient emailClient,
        IConfiguration configuration,
        ILogger<AzureEmailService> logger)
    {
        _emailClient = emailClient;
        _configuration = configuration;
        _logger = logger;
        _fromEmail = configuration["AzureEmail:FromAddress"] ?? "noreply@paddokk.com";
        _fromDisplayName = configuration["AzureEmail:FromDisplayName"] ?? "Paddokk";
    }

    public async Task SendEmailConfirmationAsync(string email, string displayName, string userId, string token)
    {
        var frontendUrl = _configuration["Frontend:BaseUrl"];
        var confirmationLink = $"{frontendUrl}/confirm-email?userId={userId}&token={UrlEncoder.Default.Encode(token)}";

        var subject = "Confirm your Paddokk account";
        var htmlContent = GenerateEmailConfirmationHtml(displayName, confirmationLink);
        var plainTextContent = GenerateEmailConfirmationText(displayName, confirmationLink);

        await SendEmailAsync(email, subject, htmlContent, plainTextContent);
    }

    public async Task SendPasswordResetEmailAsync(string email, string displayName, string token)
    {
        var frontendUrl = _configuration["Frontend:BaseUrl"];
        var resetLink = $"{frontendUrl}/reset-password?email={email}&token={UrlEncoder.Default.Encode(token)}";

        var subject = "Reset your Paddokk password";
        var htmlContent = GeneratePasswordResetHtml(displayName, resetLink);
        var plainTextContent = GeneratePasswordResetText(displayName, resetLink);

        await SendEmailAsync(email, subject, htmlContent, plainTextContent);
    }

    public async Task SendWelcomeEmailAsync(string email, string displayName)
    {
        var subject = "Welcome to Paddokk!";
        var htmlContent = GenerateWelcomeHtml(displayName);
        var plainTextContent = GenerateWelcomeText(displayName);

        await SendEmailAsync(email, subject, htmlContent, plainTextContent);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent)
    {
        try
        {
            var emailMessage = new EmailMessage(
                senderAddress: _fromEmail,
                content: new EmailContent(subject)
                {
                    PlainText = plainTextContent,
                    Html = htmlContent
                },
                recipients: new EmailRecipients(new List<EmailAddress>
                {
                        new EmailAddress(toEmail)
                }));

            // Set display name for sender
            emailMessage.Headers.Add("From", $"{_fromDisplayName} <{_fromEmail}>");

            var emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            if (emailSendOperation.HasCompleted)
            {
                _logger.LogInformation("Email sent successfully to {Email}. MessageId: {MessageId}",
                    toEmail, emailSendOperation.Id);
            }
            else
            {
                _logger.LogWarning("Email sending operation did not complete for {Email}", toEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }

    #region Email Templates

    private string GenerateEmailConfirmationHtml(string displayName, string confirmationLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Confirm your Paddokk account</title>
            <style>
                body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 0; }}
                .header {{ background-color: #506D50; color: white; padding: 30px; text-align: center; }}
                .header h1 {{ margin: 0; font-size: 28px; font-weight: 300; }}
                .content {{ padding: 40px 30px; }}
                .content h2 {{ color: #506D50; font-size: 24px; margin-bottom: 20px; }}
                .button {{ display: inline-block; background-color: #506D50; color: white; padding: 15px 30px; text-decoration: none; border-radius: 6px; font-weight: 500; font-size: 16px; margin: 20px 0; }}
                .button:hover {{ background-color: #3d5a3d; }}
                .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; }}
                .logo {{ font-size: 32px; font-weight: bold; }}
                @media (max-width: 600px) {{
                    .container {{ width: 100%; }}
                    .content {{ padding: 20px; }}
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <div class=""logo"">🏁 Paddokk</div>
                    <p style=""margin: 10px 0 0 0; opacity: 0.9;"">Your Automotive Journey Platform</p>
                </div>
                <div class=""content"">
                    <h2>Welcome to Paddokk, {displayName}!</h2>
                    <p>Thanks for joining the automotive community that shares real journeys. Before you can start documenting your builds and connecting with other enthusiasts, we need to verify your email address.</p>
            
                    <p style=""text-align: center; margin: 30px 0;"">
                        <a href=""{confirmationLink}"" class=""button"">Confirm Email Address</a>
                    </p>
            
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p style=""word-break: break-all; background-color: #f8f9fa; padding: 15px; border-radius: 4px; font-family: monospace; font-size: 14px;"">{confirmationLink}</p>
            
                    <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">
            
                    <p><strong>What's next?</strong></p>
                    <ul>
                        <li>🚗 Add your first car to your garage</li>
                        <li>🛣️ Create your first automotive journey</li>
                        <li>📸 Share updates with photos and progress</li>
                        <li>👥 Follow other builders and racers</li>
                    </ul>
            
                    <p>Thanks,<br><strong>The Paddokk Team</strong></p>
                </div>
                <div class=""footer"">
                    <p>This email was sent to verify your Paddokk account. If you didn't create an account, you can safely ignore this email.</p>
                    <p>© 2024 Paddokk. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerateEmailConfirmationText(string displayName, string confirmationLink)
    {
        return $@"Welcome to Paddokk, {displayName}!

        Thanks for joining the automotive community that shares real journeys. Before you can start documenting your builds and connecting with other enthusiasts, we need to verify your email address.

        Please confirm your email address by clicking this link:
        {confirmationLink}

        What's next?
        - Add your first car to your garage
        - Create your first automotive journey  
        - Share updates with photos and progress
        - Follow other builders and racers

        Thanks,
        The Paddokk Team

        If you didn't create an account, you can safely ignore this email.";
    }

    private string GeneratePasswordResetHtml(string displayName, string resetLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Reset your Paddokk password</title>
            <style>
                body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 0; }}
                .header {{ background-color: #506D50; color: white; padding: 30px; text-align: center; }}
                .header h1 {{ margin: 0; font-size: 28px; font-weight: 300; }}
                .content {{ padding: 40px 30px; }}
                .content h2 {{ color: #506D50; font-size: 24px; margin-bottom: 20px; }}
                .button {{ display: inline-block; background-color: #506D50; color: white; padding: 15px 30px; text-decoration: none; border-radius: 6px; font-weight: 500; font-size: 16px; margin: 20px 0; }}
                .alert {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 4px; margin: 20px 0; }}
                .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; }}
                .logo {{ font-size: 32px; font-weight: bold; }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <div class=""logo"">🏁 Paddokk</div>
                </div>
                <div class=""content"">
                    <h2>Password Reset Request</h2>
                    <p>Hi {displayName},</p>
                    <p>You requested to reset your Paddokk account password. Click the button below to set a new password:</p>
            
                    <p style=""text-align: center; margin: 30px 0;"">
                        <a href=""{resetLink}"" class=""button"">Reset Password</a>
                    </p>
            
                    <div class=""alert"">
                        <strong>⚠️ Security Notice:</strong> This link will expire in 24 hours for your security.
                    </div>
            
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p style=""word-break: break-all; background-color: #f8f9fa; padding: 15px; border-radius: 4px; font-family: monospace; font-size: 14px;"">{resetLink}</p>
            
                    <p>If you didn't request this password reset, you can safely ignore this email. Your password will remain unchanged.</p>
            
                    <p>Thanks,<br><strong>The Paddokk Team</strong></p>
                </div>
                <div class=""footer"">
                    <p>For security, this password reset link will expire in 24 hours.</p>
                    <p>© 2024 Paddokk. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GeneratePasswordResetText(string displayName, string resetLink)
    {
        return $@"Password Reset Request

        Hi {displayName},

        You requested to reset your Paddokk account password. Click this link to set a new password:
        {resetLink}

        SECURITY NOTICE: This link will expire in 24 hours for your security.

        If you didn't request this password reset, you can safely ignore this email. Your password will remain unchanged.

        Thanks,
        The Paddokk Team";
    }

    private string GenerateWelcomeHtml(string displayName)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Welcome to Paddokk!</title>
            <style>
                body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 0; }}
                .header {{ background-color: #506D50; color: white; padding: 30px; text-align: center; }}
                .content {{ padding: 40px 30px; }}
                .content h2 {{ color: #506D50; font-size: 24px; margin-bottom: 20px; }}
                .feature {{ background-color: #f8f9fa; padding: 20px; margin: 15px 0; border-radius: 6px; border-left: 4px solid #506D50; }}
                .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; }}
                .logo {{ font-size: 32px; font-weight: bold; }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <div class=""logo"">🏁 Paddokk</div>
                    <h1 style=""margin: 10px 0 0 0; font-weight: 300;"">Welcome Aboard!</h1>
                </div>
                <div class=""content"">
                    <h2>Ready to start your automotive journey, {displayName}?</h2>
                    <p>You're now part of the community that shares real automotive journeys - from weekend builds to professional racing campaigns.</p>
            
                    <div class=""feature"">
                        <h3 style=""margin-top: 0; color: #506D50;"">🚗 Your Garage</h3>
                        <p>Add your cars and document every modification, upgrade, and milestone.</p>
                    </div>
            
                    <div class=""feature"">
                        <h3 style=""margin-top: 0; color: #506D50;"">🛣️ Journey Threads</h3>
                        <p>Create ongoing stories about your builds, races, and automotive adventures.</p>
                    </div>
            
                    <div class=""feature"">
                        <h3 style=""margin-top: 0; color: #506D50;"">👥 Community</h3>
                        <p>Follow other enthusiasts, get inspired, and share your expertise.</p>
                    </div>
            
                    <p>Start by adding your first car to your garage, then create your first journey to begin sharing your automotive story!</p>
            
                    <p>Happy building!<br><strong>The Paddokk Team</strong></p>
                </div>
                <div class=""footer"">
                    <p>© 2024 Paddokk. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerateWelcomeText(string displayName)
    {
        return $@"Welcome to Paddokk, {displayName}!

        You're now part of the community that shares real automotive journeys - from weekend builds to professional racing campaigns.

        🚗 YOUR GARAGE
        Add your cars and document every modification, upgrade, and milestone.

        🛣️ JOURNEY THREADS  
        Create ongoing stories about your builds, races, and automotive adventures.

        👥 COMMUNITY
        Follow other enthusiasts, get inspired, and share your expertise.

        Start by adding your first car to your garage, then create your first journey to begin sharing your automotive story!

        Happy building!
        The Paddokk Team";
    }

    #endregion
}
