import { createStart } from "@tanstack/react-start";
import { apiErrorSerializationAdapter } from "@/lib/api/api-error-serialization";

/**
 * TanStack Start instance, auto-discovered by the Start vite plugin (the "start" entry
 * convention, alongside `src/router.tsx`). Its sole job today is registering
 * {@link apiErrorSerializationAdapter} so an {@link ApiError} thrown inside a BFF server
 * function keeps its `code`/`status`/`errors`/`traceId` across the network boundary.
 *
 * Without this, seroval falls back to the built-in error plugin (`$TSR/Error`), which
 * serializes only `message` — so the error-code resolver never sees a `code` and every
 * API failure degrades to the generic fallback toast (see ADR-0007).
 */
export const startInstance = createStart(() => ({
  serializationAdapters: [apiErrorSerializationAdapter],
}));
