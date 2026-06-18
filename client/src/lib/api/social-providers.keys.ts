/**
 * TanStack Query keys for the social-auth feature. Single source of truth so the
 * key can never drift between registration and invalidation.
 */
export const socialProvidersKeys = {
  enabled: ["social-providers", "enabled"] as const,
} as const;
