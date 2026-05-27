import { useInfiniteQuery } from "@tanstack/react-query";
import { getUserJourneysFn } from "@/lib/api/user-journeys";

const USER_JOURNEYS_PAGE_SIZE = 20;

export function useUserJourneysInfinite(enabled = true) {
  return useInfiniteQuery({
    queryKey: ["user-journeys"],
    queryFn: ({ pageParam = 1 }) =>
      getUserJourneysFn({
        data: { page: pageParam, pageSize: USER_JOURNEYS_PAGE_SIZE },
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
    enabled,
  });
}
