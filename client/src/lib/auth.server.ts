import { betterAuth } from "better-auth";
import { drizzleAdapter } from "better-auth/adapters/drizzle";
import { jwt } from "better-auth/plugins";
import { tanstackStartCookies } from "better-auth/tanstack-start";
import { db } from "./db/index.server";
import * as schema from "./db/schema";
import { sendEmail } from "./email/email.server";
import { buildChangeEmailEmail } from "./email/templates/change-email";
import { buildResetPasswordEmail } from "./email/templates/reset-password";
import { buildVerifyEmailEmail } from "./email/templates/verify-email";

const apiUrl = process.env.VITE_API_URL;
if (!apiUrl) throw new Error("VITE_API_URL is required");

const trustedOrigins = process.env.BETTER_AUTH_TRUSTED_ORIGINS
  ? process.env.BETTER_AUTH_TRUSTED_ORIGINS.split(",").map((o) => o.trim())
  : [];

export const auth = betterAuth({
  database: drizzleAdapter(db, {
    provider: "pg",
    schema,
  }),

  trustedOrigins,

  emailAndPassword: {
    enabled: true,
    requireEmailVerification: true,
    sendResetPassword: async ({ user, url }) => {
      const { subject, html, text } = buildResetPasswordEmail({
        name: user.name,
        resetUrl: url,
      });
      await sendEmail({ to: user.email, subject, html, text });
    },
  },

  emailVerification: {
    sendOnSignUp: true,
    sendVerificationEmail: async ({ user, url, token }) => {
      const appUrl = process.env.VITE_BETTER_AUTH_URL ?? new URL(url).origin;
      const verifyUrl = `${appUrl}/verify-email?token=${encodeURIComponent(token)}`;
      const { subject, html, text } = buildVerifyEmailEmail({
        name: user.name,
        verifyUrl,
      });
      await sendEmail({ to: user.email, subject, html, text });
    },
  },

  user: {
    changeEmail: {
      enabled: true,
      sendChangeEmailVerification: async ({ user, newEmail, url }) => {
        const { subject, html, text } = buildChangeEmailEmail({
          name: user.name,
          newEmail,
          verifyUrl: url,
        });
        await sendEmail({ to: user.email, subject, html, text });
      },
    },
    deleteUser: {
      enabled: true,
      beforeDelete: async (_user, request) => {
        if (!request) return;
        const tokenRes = await auth.handler(
          new Request(`${apiUrl.replace(/\/$/, "")}/api/auth/token`, {
            headers: request.headers,
          }),
        );
        if (!tokenRes.ok) return;
        const { token } = (await tokenRes.json()) as { token?: string };
        if (!token) return;

        await fetch(`${apiUrl}/api/v1/Users/me`, {
          method: "DELETE",
          headers: { Authorization: `Bearer ${token}` },
        });
      },
    },
  },

  session: {
    expiresIn: 60 * 60 * 24 * 7, // 7 days
    updateAge: 60 * 60 * 24, // Update session every 24 hours
    cookieCache: {
      enabled: true,
      maxAge: 5 * 60, // 5 minutes
    },
  },

  plugins: [tanstackStartCookies(), jwt()],
});
