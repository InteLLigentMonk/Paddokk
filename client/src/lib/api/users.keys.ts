/**
 * Single source of truth for every TanStack Query key in the users feature.
 *
 * Query definitions and `invalidateQueries` calls MUST reference these helpers
 * instead of re-typing string arrays, so a key can never drift between where it
 * is set and where it is invalidated.
 *
 * Convention:
 * - A function returns the full key for a specific query (`userKeys.userDetail(name)`).
 * - A `*Root` constant is the prefix used for broad invalidation of every query
 *   in that family (`userKeys.userDetailRoot`). TanStack matches by prefix, so
 *   invalidating the root settles all variants.
 *
 * The leaf strings here intentionally match the historical keys verbatim so the
 * runtime cache identity is unchanged by the migration to this factory.
 */

export type FollowListType = "followers" | "following";

export const userKeys = {
  currentUser: ["current-user"] as const,

  userDetailRoot: ["user-by-username"] as const,
  userDetail: (username: string) => ["user-by-username", username] as const,

  userCarsRoot: ["user-cars-by-username"] as const,
  userCars: (username: string, limit?: number) =>
    ["user-cars-by-username", username, limit ?? null] as const,

  userCarDetailRoot: ["user-car-by-slug"] as const,
  userCarDetail: (username: string, slug: string) =>
    ["user-car-by-slug", username, slug] as const,

  userJourneysRoot: ["user-journeys-by-username"] as const,
  userJourneys: (username: string) =>
    ["user-journeys-by-username", username] as const,

  journeyDetail: (username: string, slug: string) =>
    ["journey-by-slug", username, slug] as const,

  followListRoot: ["follow-list"] as const,
  followList: (type: FollowListType, userId: string) =>
    ["follow-list", type, userId] as const,
} as const;
