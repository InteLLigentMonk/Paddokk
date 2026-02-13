import { createServerFn } from "@tanstack/react-start";
import { getRequestHeaders } from "@tanstack/react-start/server";
import { auth } from "./auth";

/**
 * Server function to get the current auth session
 * Can be called from beforeLoad hooks for SSR-compatible auth checks
 */
export const getAuthSession = createServerFn({ method: "GET" }).handler(
  async () => {
    const headers = getRequestHeaders();
    const session = await auth.api.getSession({ headers });

    return {
      user: session?.user ?? null,
      session: session?.session ?? null,
    };
  },
);

/**
 * Server function to ensure user is authenticated
 * Throws an error if no session exists
 */
export const ensureSession = createServerFn({ method: "GET" }).handler(
  async () => {
    const headers = getRequestHeaders();
    const session = await auth.api.getSession({ headers });

    if (!session) {
      throw new Error("Unauthorized");
    }

    return session;
  },
);
