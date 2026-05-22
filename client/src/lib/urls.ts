/**
 * Centralized URL builders for user-scoped resources. Routes use slugs (not numeric ids)
 * everywhere user-facing — keep call sites consistent by going through these helpers.
 */

export const userProfileUrl = (username: string) =>
  `/users/${username}` as const;

export const userCarsUrl = (username: string) =>
  `/users/${username}/cars` as const;

export const carUrl = (params: { ownerUsername: string; slug: string }) =>
  `/users/${params.ownerUsername}/cars/${params.slug}` as const;

export const userJourneysUrl = (username: string) =>
  `/users/${username}/journeys` as const;

export const journeyUrl = (params: { ownerUsername: string; slug: string }) =>
  `/users/${params.ownerUsername}/journeys/${params.slug}` as const;

// Owner-only edit/create routes live under /me/...
export const newCarUrl = () => "/me/cars/new" as const;
export const editCarUrl = (slug: string) => `/me/cars/${slug}/edit` as const;
export const newJourneyUrl = () => "/me/journeys/new" as const;
export const editJourneyUrl = (slug: string) =>
  `/me/journeys/${slug}/edit` as const;
