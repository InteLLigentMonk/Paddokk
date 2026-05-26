export type EmailContent = {
  subject: string;
  html: string;
  text: string;
};

export function escapeHtml(value: string): string {
  return value
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;");
}

export function resolveGreeting(name: string, fallback = "there"): string {
  const trimmed = name.trim();
  return trimmed.length > 0 ? trimmed : fallback;
}

type LayoutInput = {
  preheader: string;
  heading: string;
  bodyHtml: string;
};

export function wrapLayout({
  preheader,
  heading,
  bodyHtml,
}: LayoutInput): string {
  return `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width,initial-scale=1" />
<title>${escapeHtml(heading)}</title>
</head>
<body style="margin:0;padding:0;background:#f4f4f5;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif;color:#18181b;">
<span style="display:none;visibility:hidden;opacity:0;color:transparent;height:0;width:0;">${escapeHtml(preheader)}</span>
<table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background:#f4f4f5;padding:32px 0;">
<tr><td align="center">
<table role="presentation" width="560" cellspacing="0" cellpadding="0" border="0" style="background:#ffffff;border-radius:8px;padding:32px;max-width:560px;width:100%;">
<tr><td>
<h1 style="margin:0 0 16px 0;font-size:22px;font-weight:600;color:#18181b;">Paddokk</h1>
${bodyHtml}
<hr style="border:none;border-top:1px solid #e4e4e7;margin:32px 0 16px 0;" />
<p style="margin:0;font-size:12px;color:#71717a;line-height:1.5;">
This message was sent by Paddokk. If you did not expect this email, you can safely ignore it.
</p>
</td></tr>
</table>
</td></tr>
</table>
</body>
</html>`;
}
