import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import { apiFetcher } from "./client";
import type {
  GetCarsBrowseStatsResponse,
  PagedUserCarsResponse,
  UserCarDto,
} from "@/generated/api/schemas";

// CarSearchSort matches backend enum ordinal: 0=Relevance, 1=Newest, 2=MostLiked, 3=MostJourneys
export const CAR_SEARCH_SORT = {
  Relevance: 0,
  Newest: 1,
  MostLiked: 2,
  MostJourneys: 3,
} as const;

export type CarSortKey = keyof typeof CAR_SEARCH_SORT;

const browseCarsSchema = z.object({
  terms: z.array(z.string().min(1).max(50)).max(10).default([]),
  sort: z.number().int().min(0).max(3).default(CAR_SEARCH_SORT.Newest),
  isPublic: z.boolean().optional(),
  page: z.number().int().min(1).default(1),
  pageSize: z.number().int().min(1).max(50).default(24),
});

const browseStatsSchema = z.object({
  terms: z.array(z.string().min(1).max(50)).max(10).default([]),
  isPublic: z.boolean().optional(),
});

const getCarSchema = z.object({ carId: z.number().int().min(1) });

export const getPublicCarFn = createServerFn({ method: "GET" })
  .inputValidator(getCarSchema)
  .handler(async ({ data: { carId } }) => {
    const result = await apiFetcher<{ data: UserCarDto }>(
      `/api/v1/cars/${carId}`,
    );
    return result.data;
  });

export const searchCarsFn = createServerFn({ method: "GET" })
  .inputValidator(browseCarsSchema)
  .handler(async ({ data: { terms, sort, isPublic, page, pageSize } }) => {
    const params = new URLSearchParams({
      sort: String(sort),
      page: String(page),
      pageSize: String(pageSize),
    });
    terms.forEach((t) => params.append("terms", t));
    if (isPublic !== undefined) params.set("isPublic", String(isPublic));
    const result = await apiFetcher<{ data: PagedUserCarsResponse }>(
      `/api/v1/cars/search?${params}`,
    );
    return result.data;
  });

export const getCarsBrowseStatsFn = createServerFn({ method: "GET" })
  .inputValidator(browseStatsSchema)
  .handler(async ({ data: { terms, isPublic } }) => {
    const params = new URLSearchParams();
    terms.forEach((t) => params.append("terms", t));
    if (isPublic !== undefined) params.set("isPublic", String(isPublic));
    const result = await apiFetcher<{ data: GetCarsBrowseStatsResponse }>(
      `/api/v1/cars/browse-stats?${params}`,
    );
    return result.data;
  });
