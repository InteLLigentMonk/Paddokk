namespace Paddokk.Core.Interfaces;

public interface IHtmlSanitizationService
{
    string? Sanitize(string? html);
}
