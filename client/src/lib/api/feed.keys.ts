/**
 * Single source of truth for the feed feature's TanStack Query keys.
 *
 * The feed is unparameterised from the client's point of view — the actor is implicit
 * (resolved server-side), so there is one infinite query and one root key.
 */
export const feedKeys = {
  root: ["feed"] as const,
} as const;
