using System.Globalization;
using System.Text;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Common;

public sealed class UsernameGenerator
{
    private const int MinLength = 2;
    private const int MaxLength = 30;
    private const int MaxAttempts = 100;

    public static readonly IReadOnlySet<string> Reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "me", "admin", "administrator", "api", "auth", "user", "users",
        "signup", "signin", "signout", "login", "logout", "register",
        "settings", "profile", "dashboard", "explore", "search", "feed",
        "help", "about", "support", "contact", "privacy", "terms",
        "marketplace", "stores", "events", "community", "knowledge",
        "cars", "journeys", "gallery", "inventory", "notifications",
        "subscription", "subscriptions"
    };

    public string Generate(string firstName, string? lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);

        var first = Sanitize(firstName);
        var last = string.IsNullOrWhiteSpace(lastName) ? string.Empty : Sanitize(lastName);

        var candidate = last.Length > 0 ? $"{first}.{last}" : first;

        if (candidate.Length < MinLength || first.Length == 0)
            throw new ArgumentException(
                $"Cannot generate username from '{firstName} {lastName}' — no valid characters remain after sanitization",
                nameof(firstName));

        return candidate.Length > MaxLength ? candidate[..MaxLength] : candidate;
    }

    public async Task<string> EnsureUniqueAsync(
        string candidate,
        IUserRepository repository,
        CancellationToken cancellationToken)
    {
        var probe = candidate;

        for (var suffix = 0; suffix <= MaxAttempts; suffix++)
        {
            if (suffix > 0)
            {
                var suffixStr = $".{suffix}";
                var baseName = candidate.Length + suffixStr.Length > MaxLength
                    ? candidate[..(MaxLength - suffixStr.Length)]
                    : candidate;
                probe = $"{baseName}{suffixStr}";
            }

            var conflicts = Reserved.Contains(probe)
                || await repository.UsernameExistsAsync(probe, cancellationToken);

            if (!conflicts) return probe;
        }

        throw new InvalidOperationException(
            $"Could not generate unique username from base '{candidate}' after {MaxAttempts} attempts");
    }

    private static string Sanitize(string input)
    {
        var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                sb.Append(c);
        }

        return sb.ToString();
    }
}
