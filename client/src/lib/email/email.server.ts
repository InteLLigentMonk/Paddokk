import { Resend } from "resend";

const DEFAULT_FROM_ADDRESS = "Paddokk <onboarding@resend.dev>";

export type SendEmailInput = {
  to: string;
  subject: string;
  html: string;
  text: string;
};

let cachedClient: Resend | null = null;

function getClient(): Resend | null {
  const apiKey = process.env.RESEND_API_KEY;
  if (!apiKey) {
    return null;
  }
  if (cachedClient === null) {
    cachedClient = new Resend(apiKey);
  }
  return cachedClient;
}

function getFromAddress(): string {
  return process.env.EMAIL_FROM_ADDRESS ?? DEFAULT_FROM_ADDRESS;
}

export async function sendEmail(input: SendEmailInput): Promise<void> {
  const client = getClient();

  if (!client) {
    console.warn(
      "[email] RESEND_API_KEY is not set. Falling back to console output.",
    );
    console.log(
      `[email] (dev) To: ${input.to} | Subject: ${input.subject}\n${input.text}`,
    );
    return;
  }

  const { error } = await client.emails.send({
    from: getFromAddress(),
    to: [input.to],
    subject: input.subject,
    html: input.html,
    text: input.text,
  });

  if (error) {
    throw new Error(`Resend send failed: ${error.message}`);
  }
}
