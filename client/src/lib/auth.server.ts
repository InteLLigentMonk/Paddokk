import { betterAuth } from "better-auth";
import { drizzleAdapter } from "better-auth/adapters/drizzle";
import { jwt } from "better-auth/plugins";
import { tanstackStartCookies } from "better-auth/tanstack-start";
import { db } from "./db/index.server";
import * as schema from "./db/schema";

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
    requireEmailVerification: false, // Set to true when email service is integrated
    sendResetPassword: async ({ user, url }) => {
      // TODO: Integrate email service (Resend, SendGrid, etc.)
      console.log(`[DEV] Password reset link for ${user.email}: ${url}`);
    },
  },

  emailVerification: {
    sendOnSignUp: true,
    sendVerificationEmail: async ({ user, url }) => {
      // TODO: Integrate email service
      console.log(`[DEV] Verification email for ${user.email}: ${url}`);
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
