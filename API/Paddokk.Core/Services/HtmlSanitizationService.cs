using Ganss.Xss;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Services;

public sealed class HtmlSanitizationService : IHtmlSanitizationService
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.UnionWith(["p", "br", "strong", "em", "u", "h2", "h3", "ul", "ol", "li", "a"]);

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.UnionWith(["href", "target", "rel"]);

        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.UnionWith(["http", "https", "mailto"]);

        return sanitizer;
    }

    public string? Sanitize(string? html)
    {
        if (string.IsNullOrEmpty(html)) return html;
        return Sanitizer.Sanitize(html);
    }
}
