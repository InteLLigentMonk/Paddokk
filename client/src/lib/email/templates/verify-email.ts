import { escapeHtml, resolveGreeting, wrapLayout } from "./_layout";
import type { EmailContent } from "./_layout";

export type VerifyEmailInput = {
  name: string;
  verifyUrl: string;
};

export function buildVerifyEmailEmail({
  name,
  verifyUrl,
}: VerifyEmailInput): EmailContent {
  const greeting = resolveGreeting(name);
  const safeGreeting = escapeHtml(greeting);
  const safeUrl = escapeHtml(verifyUrl);

  const subject = "Verify your Paddokk email address";

  const bodyHtml = `
<h2 style="margin:0 0 16px 0;font-size:18px;font-weight:600;">Welcome, ${safeGreeting}!</h2>
<p style="margin:0 0 16px 0;font-size:15px;line-height:1.5;color:#3f3f46;">
Thanks for creating a Paddokk account. To activate your account, please verify your email address by clicking the button below.
</p>
<p style="margin:24px 0;">
<a href="${safeUrl}" style="background:#18181b;color:#ffffff;text-decoration:none;padding:12px 24px;border-radius:6px;font-size:15px;font-weight:500;display:inline-block;">Verify email</a>
</p>
<p style="margin:0 0 16px 0;font-size:13px;color:#71717a;line-height:1.5;">
If the button does not work, copy and paste this URL into your browser:<br />
<a href="${safeUrl}" style="color:#3f3f46;word-break:break-all;">${safeUrl}</a>
</p>
<p style="margin:0;font-size:13px;color:#71717a;line-height:1.5;">
If you did not create a Paddokk account, you can safely ignore this email.
</p>
`;

  const html = wrapLayout({
    preheader: "Verify your Paddokk email address",
    heading: subject,
    bodyHtml,
  });

  const text = `Welcome, ${greeting}!

Thanks for creating a Paddokk account. Please verify your email address by opening the link below:

${verifyUrl}

If you did not create a Paddokk account, you can safely ignore this email.

— Paddokk`;

  return { subject, html, text };
}
