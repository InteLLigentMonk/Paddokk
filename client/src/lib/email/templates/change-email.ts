import { escapeHtml, resolveGreeting, wrapLayout } from "./_layout";
import type { EmailContent } from "./_layout";

export type ChangeEmailInput = {
  name: string;
  newEmail: string;
  verifyUrl: string;
};

export function buildChangeEmailEmail({
  name,
  newEmail,
  verifyUrl,
}: ChangeEmailInput): EmailContent {
  const greeting = resolveGreeting(name);
  const safeGreeting = escapeHtml(greeting);
  const safeUrl = escapeHtml(verifyUrl);
  const safeNewEmail = escapeHtml(newEmail);

  const subject = "Confirm your new Paddokk email address";

  const bodyHtml = `
<h2 style="margin:0 0 16px 0;font-size:18px;font-weight:600;">Hi ${safeGreeting},</h2>
<p style="margin:0 0 16px 0;font-size:15px;line-height:1.5;color:#3f3f46;">
You requested to change the email address on your Paddokk account to <strong>${safeNewEmail}</strong>. Click the button below to confirm this change.
</p>
<p style="margin:24px 0;">
<a href="${safeUrl}" style="background:#18181b;color:#ffffff;text-decoration:none;padding:12px 24px;border-radius:6px;font-size:15px;font-weight:500;display:inline-block;">Confirm new email</a>
</p>
<p style="margin:0 0 16px 0;font-size:13px;color:#71717a;line-height:1.5;">
If the button does not work, copy and paste this URL into your browser:<br />
<a href="${safeUrl}" style="color:#3f3f46;word-break:break-all;">${safeUrl}</a>
</p>
<p style="margin:0;font-size:13px;color:#71717a;line-height:1.5;">
If you did not request this change, you can safely ignore this email. Your address will stay the same.
</p>
`;

  const html = wrapLayout({
    preheader: "Confirm your new Paddokk email address",
    heading: subject,
    bodyHtml,
  });

  const text = `Hi ${greeting},

You requested to change your Paddokk email address to ${newEmail}. Confirm this change by opening the link below:

${verifyUrl}

If you did not request this change, you can safely ignore this email.

— Paddokk`;

  return { subject, html, text };
}
