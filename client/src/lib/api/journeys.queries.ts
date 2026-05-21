import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import {
  JOURNEY_SEARCH_SORT,
  
  getJourneysBrowseStatsFn,
  likeJourneyFn,
  searchJourneysFn,
  subscribeToJourneyFn,
  unlikeJourneyFn,
  unsubscribeFromJourneyFn
} from "./journeys";
import type {JourneySortKey} from "./journeys";
import type { InfiniteData } from "@tanstack/react-query";
import type { PagedJourneysResponse } from "@/generated/api/schemas";

const PAGE_SIZE = 24;

export const browseJourneysInfiniteQueryOptions = (terms: Array<string>, sort: number) =>
  infiniteQueryOptions({
    queryKey: ["browse-journeys", { terms, sort }] as const,
    queryFn: ({ pageParam }) =>
      searchJourneysFn({
        data: { terms, sort, page: pageParam, pageSize: PAGE_SIZE },
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasMore ? allPages.length + 1 : undefined,
  });

export const browseJourneysStatsQueryOptions = (terms: Array<string>) =>
  queryOptions({
    queryKey: ["browse-journeys-stats", { terms }] as const,
    queryFn: () => getJourneysBrowseStatsFn({ data: { terms } }),
  });

export function sortKeyToNumber(key: JourneySortKey | undefined, hasTerms: boolean): number {
  if (key === undefined) return hasTerms ? JOURNEY_SEARCH_SORT.RecentActivity : JOURNEY_SEARCH_SORT.Newest;
  return JOURNEY_SEARCH_SORT[key];
}

function patchJourneyInPages(
  data: InfiniteData<PagedJourneysResponse>,
  journeyId: number | string,
  patch: (journey: PagedJourneysResponse["journeys"][number]) => PagedJourneysResponse["journeys"][number],
): InfiniteData<PagedJourneysResponse> {
  return {
    ...data,
    pages: data.pages.map((page) => ({
      ...page,
      journeys: page.journeys.map((j) => (j.id == journeyId ? patch(j) : j)),
    })),
  };
}

export function useToggleJourneyLike(journeyId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (isCurrentlyLiked: boolean) =>
      isCurrentlyLiked
        ? unlikeJourneyFn({ data: { journeyId } })
        : likeJourneyFn({ data: { journeyId } }),
    onMutate: async (isCurrentlyLiked) => {
      await queryClient.cancelQueries({ queryKey: ["browse-journeys"] });
      const snapshots = queryClient.getQueriesData<InfiniteData<PagedJourneysResponse>>({
        queryKey: ["browse-journeys"],
      });
      snapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(
          key,
          patchJourneyInPages(data, journeyId, (j) => ({
            ...j,
            isLiked: !isCurrentlyLiked,
            likeCount: Number(j.likeCount) + (isCurrentlyLiked ? -1 : 1),
          })),
        );
      });
      return { snapshots };
    },
    onError: (_err, _vars, context) => {
      context?.snapshots.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      notifications.show({
        color: "red",
        message: "Kunde inte uppdatera gillamarkering. FÃ¶rsÃ¶k igen.",
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["browse-journeys"] });
    },
  });
}

export function useToggleJourneySubscription(journeyId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (isCurrentlySubscribed: boolean) =>
      isCurrentlySubscribed
        ? unsubscribeFromJourneyFn({ data: { journeyId } })
        : subscribeToJourneyFn({ data: { journeyId } }),
    onMutate: async (isCurrentlySubscribed) => {
      await queryClient.cancelQueries({ queryKey: ["browse-journeys"] });
      const snapshots = queryClient.getQueriesData<InfiniteData<PagedJourneysResponse>>({
        queryKey: ["browse-journeys"],
      });
      snapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(
          key,
          patchJourneyInPages(data, journeyId, (j) => ({
            ...j,
            isSubscribed: !isCurrentlySubscribed,
            subscriberCount: Number(j.subscriberCount) + (isCurrentlySubscribed ? -1 : 1),
          })),
        );
      });
      return { snapshots };
    },
    onError: (_err, _vars, context) => {
      context?.snapshots.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      notifications.show({
        color: "red",
        message: "Kunde inte uppdatera prenumeration. FÃ¶rsÃ¶k igen.",
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["browse-journeys"] });
    },
  });
}
