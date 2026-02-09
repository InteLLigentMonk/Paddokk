namespace API.Services;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string displayName, string userId, string token);
    Task SendPasswordResetEmailAsync(string email, string displayName, string token);
    Task SendWelcomeEmailAsync(string email, string displayName);
}
