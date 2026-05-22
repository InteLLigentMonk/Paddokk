import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";
import { JourneysBrowsePage } from "@/components/journeys/journeys-browse-page";
import {
  browseJourneysInfiniteQueryOptions,
  browseJourneysStatsQueryOptions,
  sortKeyToNumber,
} from "@/lib/api/journeys.queries";

const searchSchema = z.object({
  q: z.array(z.string().min(1)).max(10).optional(),
  sort: z
    .enum([
      "RecentActivity",
      "Newest",
      "MostLiked",
      "MostSubscribed",
      "RecentlyCompleted",
    ])
    .optional(),
});

export const Route = createFileRoute("/_app/journeys/")({
  validateSearch: searchSchema,
  loaderDeps: ({ search }) => ({ q: search.q ?? [], sort: search.sort }),
  loader: async ({ deps, context: { queryClient } }) => {
    const sortNum = sortKeyToNumber(deps.sort, deps.q.length > 0);
    await Promise.all([
      queryClient.prefetchInfiniteQuery(
        browseJourneysInfiniteQueryOptions(deps.q, sortNum),
      ),
      queryClient.ensureQueryData(browseJourneysStatsQueryOptions(deps.q)),
    ]);
  },
  component: () => <JourneysBrowsePage />,
});
