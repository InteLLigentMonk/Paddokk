import { betterAuth } from "better-auth";
import { drizzleAdapter } from "better-auth/adapters/drizzle";
import { APIError } from "better-auth/api";
import { jwt } from "better-auth/plugins";
import { tanstackStartCookies } from "better-auth/tanstack-start";
import { db } from "./db/index.server";
import { getSocialProviderCredentials } from "./auth/social-config";
import { deleteApiUser } from "./delete-api-user";
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

// Only register providers whose client id + secret are both present. Buttons
// for unconfigured providers are disabled in the UI via the enabled-providers
// server function, so the two stay in sync.
const credentials = getSocialProviderCredentials();
const socialProviders = {
  ...(credentials.google ? { google: credentials.google } : {}),
  ...(credentials.facebook ? { facebook: credentials.facebook } : {}),
};

export const auth = betterAuth({
  database: drizzleAdapter(db, {
    provider: "pg",
    schema,
  }),

  trustedOrigins,

  socialProviders,

  account: {
    accountLinking: {
      enabled: true,
      // Facebook's Graph API exposes no `email_verified` flag, so better-auth
      // treats the FB email as unverified and would refuse same-email linking
      // (account_not_linked). Facebook does require confirming an email before
      // it is added to an account, so we trust it for linking. Google is NOT
      // listed because it already returns a verified email and needs no
      // exception. GitHub/Apple etc. are intentionally absent.
      trustedProviders: ["facebook"],
      // The existing *local* account must still have a verified email before a
      // social login can link into it (default true since 1.6.11). This keeps
      // the protective half of the policy for issue #204: a social login never
      // links into an unverified pre-registered account (hijack-safe).
      requireLocalEmailVerified: true,
    },
  },

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
      sendChangeEmailConfirmation: async ({ user, newEmail, url }) => {
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
      // Anonymise the user on the .NET API BEFORE BetterAuth hard-deletes its
      // own records. Every failure path throws an APIError so the deletion is
      // aborted rather than leaving orphaned PII on the API side.
      beforeDelete: async (_user, request) => {
        if (!request) {
          throw new APIError("INTERNAL_SERVER_ERROR", {
            message: "Could not delete account: missing request context",
          });
        }
        const tokenRes = await auth.handler(
          new Request(`${apiUrl.replace(/\/$/, "")}/api/auth/token`, {
            headers: request.headers,
          }),
        );
        if (!tokenRes.ok) {
          throw new APIError("INTERNAL_SERVER_ERROR", {
            message: `Could not delete account: token request failed (${tokenRes.status})`,
          });
        }
        const { token } = (await tokenRes.json()) as { token?: string };

        await deleteApiUser({ apiUrl, token: token ?? null });
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

  plugins: [jwt(), tanstackStartCookies()],
});
