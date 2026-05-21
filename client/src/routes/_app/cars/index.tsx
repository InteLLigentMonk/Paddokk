import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";
import type { CarSortKey } from "@/lib/api/cars";
import { CarsBrowsePage } from "@/components/cars/cars-browse-page";
import {
  browseCarsInfiniteQueryOptions,
  browseCarsStatsQueryOptions,
  sortKeyToNumber,
} from "@/lib/api/cars.queries";

const searchSchema = z.object({
  q: z.array(z.string().min(1)).max(10).optional(),
  sort: z.enum(["Relevance", "Newest", "MostLiked", "MostJourneys"]).optional(),
});

export const Route = createFileRoute("/_app/cars/")({
  validateSearch: searchSchema,
  loaderDeps: ({ search }) => ({ q: search.q ?? [], sort: search.sort }),
  loader: async ({ deps, context: { queryClient } }) => {
    const sortNum = sortKeyToNumber(deps.sort, deps.q.length > 0);
    await Promise.all([
      queryClient.prefetchInfiniteQuery(browseCarsInfiniteQueryOptions(deps.q, sortNum)),
      queryClient.ensureQueryData(browseCarsStatsQueryOptions(deps.q)),
    ]);
  },
  component: () => <CarsBrowsePage />,
});
