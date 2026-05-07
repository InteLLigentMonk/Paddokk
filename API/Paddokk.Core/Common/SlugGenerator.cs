using System.Globalization;
using System.Text;

namespace Paddokk.Core.Common;

public sealed class SlugGenerator
{
    private const int MaxLength = 80;
    private const int MaxAttempts = 100;

    public string Generate(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var slug = Sanitize(input);

        if (slug.Length == 0)
            throw new ArgumentException(
                $"Cannot generate slug from '{input}' — no valid characters remain after sanitization",
                nameof(input));

        if (slug.Length > MaxLength)
            slug = slug[..MaxLength].TrimEnd('-');

        return slug;
    }

    public async Task<string> EnsureUniqueAsync(
        string candidate,
        string principalId,
        Func<string, string, CancellationToken, Task<bool>> existsAsync,
        CancellationToken cancellationToken)
    {
        var probe = candidate;

        for (var suffix = 1; suffix <= MaxAttempts; suffix++)
        {
            if (suffix > 1)
            {
                var suffixStr = $"-{suffix}";
                var baseName = candidate.Length + suffixStr.Length > MaxLength
                    ? candidate[..(MaxLength - suffixStr.Length)].TrimEnd('-')
                    : candidate;
                probe = $"{baseName}{suffixStr}";
            }

            if (!await existsAsync(principalId, probe, cancellationToken))
                return probe;
        }

        throw new InvalidOperationException(
            $"Could not generate unique slug from base '{candidate}' for principal '{principalId}' after {MaxAttempts} attempts");
    }

    private static string Sanitize(string input)
    {
        var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        var lastWasDash = true; // Treat string start as if preceded by dash to avoid leading dash

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            // Elide apostrophes silently so "Åsa's" → "asas" rather than "asa-s"
            if (c is '\'' or '’' or '`' or '´')
                continue;

            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                sb.Append(c);
                lastWasDash = false;
            }
            else if (!lastWasDash)
            {
                sb.Append('-');
                lastWasDash = true;
            }
        }

        return sb.ToString().TrimEnd('-');
    }
}
