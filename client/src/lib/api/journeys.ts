import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import { apiFetcher } from "./client";
import type {
  GetJourneysBrowseStatsResponse,
  PagedJourneysResponse,
} from "@/generated/api/schemas";
import {
  journeysLikeJourney,
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

const browseJourneysSchema = z.object({
  terms: z.array(z.string().min(1).max(50)).max(10).default([]),
  sort: z.number().int().min(1).max(5).default(JOURNEY_SEARCH_SORT.RecentActivity),
  page: z.number().int().min(1).default(1),
  pageSize: z.number().int().min(1).max(50).default(24),
});

const browseJourneysStatsSchema = z.object({
  terms: z.array(z.string().min(1).max(50)).max(10).default([]),
});

const journeyIdSchema = z.object({ journeyId: z.coerce.number().int().min(1) });

export const searchJourneysFn = createServerFn({ method: "GET" })
  .inputValidator(browseJourneysSchema)
  .handler(async ({ data: { terms, sort, page, pageSize } }) => {
    const params = new URLSearchParams({
      SortBy: String(sort),
      Page: String(page),
      PageSize: String(pageSize),
    });
    terms.forEach((t) => params.append("Terms", t));
    const result = await apiFetcher<{ data: PagedJourneysResponse }>(
      `/api/v1/journeys?${params}`,
    );
    return result.data;
  });

export const getJourneysBrowseStatsFn = createServerFn({ method: "GET" })
  .inputValidator(browseJourneysStatsSchema)
  .handler(async ({ data: { terms } }) => {
    const params = new URLSearchParams();
    terms.forEach((t) => params.append("Terms", t));
    const result = await apiFetcher<{ data: GetJourneysBrowseStatsResponse }>(
      `/api/v1/journeys/browse-stats?${params}`,
    );
    return result.data;
  });

export const likeJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await journeysLikeJourney(journeyId);
  });

export const unlikeJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await journeysUnlikeJourney(journeyId);
  });

export const subscribeToJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await journeysSubscribeToJourney(journeyId);
  });

export const unsubscribeFromJourneyFn = createServerFn({ method: "POST" })
  .inputValidator(journeyIdSchema)
  .handler(async ({ data: { journeyId } }) => {
    await journeysUnsubscribeFromJourney(journeyId);
  });
