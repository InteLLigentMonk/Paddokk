namespace Paddokk.Api.Middleware;

/// <summary>
/// Sets standard browser security headers on every response.
/// CSP is sent in report-only mode while we collect violation data;
/// switching to enforce is tracked separately (PRD #164).
/// </summary>
public class SecurityHeadersMiddleware(RequestDelegate next)
{
    private const string StrictTransportSecurityValue = "max-age=31536000; includeSubDomains";
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: blob: https:; " +
        "font-src 'self' data:; " +
        "connect-src 'self' https:; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'";

    private readonly RequestDelegate _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["Strict-Transport-Security"] = StrictTransportSecurityValue;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Content-Security-Policy-Report-Only"] = ContentSecurityPolicy;

        return _next(context);
    }
}
