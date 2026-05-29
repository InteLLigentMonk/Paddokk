/**
 * Single source of truth for the auth feature's TanStack Query keys.
 *
 * Query definitions and `removeQueries` calls MUST reference this factory
 * instead of re-typing the string array, so the key can never drift between
 * where the session query is registered and where it is removed.
 *
 * The leaf string matches the historical key verbatim so the runtime cache
 * identity is unchanged by the migration to this factory.
 */

export const authKeys = {
  session: ["auth-session"] as const,
} as const;
