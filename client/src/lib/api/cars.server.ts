import { createServerFn } from "@tanstack/react-start";
import { z } from "zod";
import { apiFetcher } from "./client";
import type { UserCarsResponse } from "@/generated/api/schemas";

const searchCarsSchema = z.object({
  q: z.string().min(1),
  page: z.number().int().min(1).default(1),
  pageSize: z.number().int().min(1).max(50).default(20),
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
