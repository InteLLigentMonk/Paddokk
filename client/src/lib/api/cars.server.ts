import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import { apiFetcher } from "./client";
import type { UserCarDto, UserCarsResponse } from "@/generated/api/schemas";

const searchCarsSchema = z.object({
  q: z.string().min(1),
  page: z.number().int().min(1).default(1),
  pageSize: z.number().int().min(1).max(50).default(20),
});

const getCarSchema = z.object({ carId: z.number().int().min(1) });

export const getPublicCarFn = createServerFn({ method: "GET" })
  .inputValidator(getCarSchema)
  .handler(async ({ data: { carId } }) => {
    const result = await apiFetcher<{ data: UserCarDto }>(`/api/v1/cars/${carId}`);
    return result.data;
  });

export const searchCarsFn = createServerFn({ method: "GET" })
  .inputValidator(searchCarsSchema)
  .handler(async ({ data: { q, page, pageSize } }) => {
    const params = new URLSearchParams({ q, page: String(page), pageSize: String(pageSize) });
    const result = await apiFetcher<{ data: UserCarsResponse }>(
      `/api/v1/cars/search?${params}`,
    );
    return result.data;
  });
