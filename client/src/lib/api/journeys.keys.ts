import { userKeys } from "./users.keys";

/**
 * Single source of truth for every TanStack Query key in the journeys feature.
 *
 * Query definitions and `invalidateQueries` / `cancelQueries` calls MUST
 * reference these helpers instead of re-typing string arrays, so a key can
 * never drift between where it is set and where it is invalidated.
 *
 * Convention:
 * - A function returns the full key for a specific query (`journeyKeys.detail(id)`).
 * - A `*Root` constant is the prefix used for broad invalidation of every query
 *   in that family (`journeyKeys.browseRoot`). TanStack matches by prefix, so
 *   invalidating the root settles all variants.
 *
 * The leaf strings here intentionally match the historical keys verbatim so the
 * runtime cache identity is unchanged by the migration to this factory.
 */

const userJourneysRoot = ["user-journeys"] as const;

export const journeyKeys = {
  browseRoot: ["browse-journeys"] as const,
  browse: (terms: Array<string>, sort: number) =>
    ["browse-journeys", { terms, sort }] as const,

  browseStats: (terms: Array<string>) =>
    ["browse-journeys-stats", { terms }] as const,

  // The current user's own journey list (infinite, unparameterised).
  userJourneysRoot,

  userJourney: (journeyId: number | null | undefined) =>
    ["user-journey", journeyId] as const,

  detail: (journeyId: number) => ["journey-detail", journeyId] as const,

  posts: (journeyId: number) => ["journey-posts", journeyId] as const,

  postComments: (postId: number) => ["post-comments", postId] as const,

  defaultActiveJourney: ["default-active-journey"] as const,

  journeyLimits: ["journey-limits"] as const,

  /**
   * The query families that together list a user's journeys: their own list
   * (`user-journeys`) and the journeys shown on a profile
   * (`user-journeys-by-username`, owned by the users feature). Mutations that
   * add/remove/update a user's journeys invalidate every root in this group.
   */
  userJourneyListRoots: [userJourneysRoot, userKeys.userJourneysRoot] as const,
} as const;
