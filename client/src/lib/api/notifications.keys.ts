/**
 * Single source of truth for the notifications feature's TanStack Query keys.
 *
 * The actor is implicit (resolved server-side), so keys are parameterised only by the
 * caller-visible inputs: the unread filter and (for the badge) the unread-count endpoint.
 */
export const notificationKeys = {
  root: ["notifications"] as const,
  list: (unread?: boolean) => ["notifications", "list", { unread }] as const,
  unreadCount: ["notifications", "unread-count"] as const,
} as const;
