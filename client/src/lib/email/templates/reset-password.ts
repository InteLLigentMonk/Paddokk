import { escapeHtml, resolveGreeting, wrapLayout } from "./_layout";
import type { EmailContent } from "./_layout";

export type ResetPasswordInput = {
  name: string;
  resetUrl: string;
};

export function buildResetPasswordEmail({
  name,
  resetUrl,
}: ResetPasswordInput): EmailContent {
  const greeting = resolveGreeting(name);
  const safeGreeting = escapeHtml(greeting);
  const safeUrl = escapeHtml(resetUrl);

  const subject = "Reset your Paddokk password";

  const bodyHtml = `
<h2 style="margin:0 0 16px 0;font-size:18px;font-weight:600;">Hi ${safeGreeting},</h2>
<p style="margin:0 0 16px 0;font-size:15px;line-height:1.5;color:#3f3f46;">
We received a request to reset your Paddokk password. Click the button below to choose a new one. The link is valid for a limited time.
</p>
<p style="margin:24px 0;">
<a href="${safeUrl}" style="background:#18181b;color:#ffffff;text-decoration:none;padding:12px 24px;border-radius:6px;font-size:15px;font-weight:500;display:inline-block;">Reset password</a>
</p>
<p style="margin:0 0 16px 0;font-size:13px;color:#71717a;line-height:1.5;">
If the button does not work, copy and paste this URL into your browser:<br />
<a href="${safeUrl}" style="color:#3f3f46;word-break:break-all;">${safeUrl}</a>
</p>
<p style="margin:0;font-size:13px;color:#71717a;line-height:1.5;">
If you did not request a password reset, you can safely ignore this email.
</p>
`;

  const html = wrapLayout({
    preheader: "Reset your Paddokk password",
    heading: subject,
    bodyHtml,
  });

  const text = `Hi ${greeting},

We received a request to reset your Paddokk password. Open the link below to choose a new one:

${resetUrl}

If you did not request a password reset, you can safely ignore this email.

— Paddokk`;

  return { subject, html, text };
}
