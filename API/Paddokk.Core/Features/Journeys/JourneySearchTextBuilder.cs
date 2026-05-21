using System.Text.RegularExpressions;

namespace Paddokk.Core.Features.Journeys;

public static class JourneySearchTextBuilder
{
    private static readonly Regex HtmlTagPattern = new("<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex WhitespacePattern = new(@"\s+", RegexOptions.Compiled);

    public static string Build(
        string? title,
        string? description,
        string? makeName,
        string? modelName,
        string? nickname,
        string? username,
        string? displayName)
    {
        var parts = new[] { title, StripHtml(description), makeName, modelName, nickname, username, displayName }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(" ", parts);
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return null;
        var stripped = HtmlTagPattern.Replace(html, " ");
        return WhitespacePattern.Replace(stripped, " ").Trim();
    }
}
