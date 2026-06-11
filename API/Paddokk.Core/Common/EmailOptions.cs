namespace Paddokk.Core.Common;

/// <summary>
/// App-wide email configuration, bound from the "Email" section. The Resend API key is read at
/// registration time (see service wiring); this class carries the values handlers need at send time.
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    // Sender shown on outgoing mail, already in "Name &lt;address&gt;" form (e.g.
    // "Paddokk &lt;noreply@mail.paddokk.com&gt;"). Must be on a Resend-verified domain in production.
    public string FromAddress { get; set; } = "Paddokk <noreply@mail.paddokk.com>";
}
