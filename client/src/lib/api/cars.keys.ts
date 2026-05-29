import { userKeys } from "./users.keys";

/**
 * Single source of truth for every TanStack Query key in the cars feature.
 *
 * Query definitions and `invalidateQueries` / `cancelQueries` calls MUST
 * reference these helpers instead of re-typing string arrays, so a key can
 * never drift between where it is set and where it is invalidated.
 *
 * Convention:
 * - A function returns the full key for a specific query (`carKeys.userCar(id)`).
 * - A `*Root` constant is the prefix used for broad invalidation of every query
 *   in that family (`carKeys.browseCarsRoot`). TanStack matches by prefix, so
 *   invalidating the root settles all variants.
 *
 * The leaf strings here intentionally match the historical keys verbatim so the
 * runtime cache identity is unchanged by the migration to this factory.
 */

const userCarsRoot = ["user-cars"] as const;

export const carKeys = {
  browseCarsRoot: ["browse-cars"] as const,
  browseCars: (terms: Array<string>, sort: number) =>
    ["browse-cars", { terms, sort }] as const,

  browseCarsStats: (terms: Array<string>) =>
    ["browse-cars-stats", { terms }] as const,

  // The current user's own garage list (infinite, unparameterised).
  userCarsRoot,

  userCar: (carId: number | null | undefined) => ["user-car", carId] as const,

  carLimits: ["car-limits"] as const,

  carMakes: ["car-makes"] as const,
  carModels: (makeId: number | null | undefined) =>
    ["car-models", makeId] as const,
  carGenerations: (modelId: number | null | undefined) =>
    ["car-generations", modelId] as const,

  // Image upload limits are shared by the car-image and journey-post flows.
  imageLimits: ["image-limits"] as const,

  carImages: (carId: number) => ["car-images", carId] as const,

  carJourneys: (username: string, carSlug: string, page = 1, pageSize = 5) =>
    ["car-journeys", username, carSlug, page, pageSize] as const,

  /**
   * The query families that together list a user's cars: their own garage
   * (`user-cars`) and the cars shown on a profile (`user-cars-by-username`,
   * owned by the users feature). Mutations that add/remove/reorder a user's
   * cars invalidate every root in this group.
   */
  userCarListRoots: [userCarsRoot, userKeys.userCarsRoot] as const,
} as const;
