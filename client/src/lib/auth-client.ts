import { createAuthClient } from "better-auth/react";
import { jwtClient } from "better-auth/client/plugins";

/**
 * Validate required environment variables
 */
const baseURL = import.meta.env.VITE_BETTER_AUTH_URL;

if (!baseURL) {
  throw new Error("VITE_BETTER_AUTH_URL environment variable is required");
}

export const authClient = createAuthClient({
  baseURL,
  plugins: [jwtClient()],
});

export const { signIn, signOut, signUp, useSession } = authClient;
