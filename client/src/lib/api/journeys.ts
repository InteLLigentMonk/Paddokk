import { createServerFn } from "@tanstack/react-start";
import {
  JourneysGetBrowseStatsQueryParams,
  JourneysLikeJourneyParams,
  JourneysSearchJourneysQueryParams,
  JourneysSubscribeToJourneyParams,
  JourneysUnlikeJourneyParams,
  JourneysUnsubscribeFromJourneyParams,
} from "@/generated/api-zod/journeys/journeys.zod";
import {
  journeysGetBrowseStats,
  journeysLikeJourney,
  journeysSearchJourneys,
  journeysSubscribeToJourney,
  journeysUnlikeJourney,
  journeysUnsubscribeFromJourney,
} from "@/generated/api/journeys/journeys";

// JourneySortBy matches backend enum ordinal: 1=RecentActivity, 2=Newest, 3=MostLiked, 4=MostSubscribed, 5=RecentlyCompleted
export const JOURNEY_SEARCH_SORT = {
  RecentActivity: 1,
  Newest: 2,
  MostLiked: 3,
  MostSubscribed: 4,
  RecentlyCompleted: 5,
} as const;

export type JourneySortKey = keyof typeof JOURNEY_SEARCH_SORT;

export const searchJourneysFn = createServerFn({ method: "GET" })
  .inputValidator(JourneysSearchJourneysQueryParams)
  .handler(async ({ data }) => await journeysSearchJourneys(data));

export const getJourneysBrowseStatsFn = createServerFn({ method: "GET" })
  .inputValidator(JourneysGetBrowseStatsQueryParams)
  .handler(async ({ data }) => await journeysGetBrowseStats(data));

export const likeJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(JourneysLikeJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await journeysLikeJourney(journeyId);
  });

export const unlikeJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(JourneysUnlikeJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await journeysUnlikeJourney(journeyId);
  });

export const subscribeToJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(JourneysSubscribeToJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await journeysSubscribeToJourney(journeyId);
  });

export const unsubscribeFromJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(JourneysUnsubscribeFromJourneyParams)
  .handler(async ({ data: { journeyId } }) => {
    await journeysUnsubscribeFromJourney(journeyId);
  });
