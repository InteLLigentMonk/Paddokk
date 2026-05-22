import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import { CAR_SEARCH_SORT, getCarsBrowseStatsFn, searchCarsFn } from "./cars";
import {
  likeUserCarFn,
  subscribeToUserCarFn,
  unlikeUserCarFn,
  unsubscribeFromUserCarFn,
} from "./user-cars";
import type { CarSortKey } from "./cars";
import type { InfiniteData } from "@tanstack/react-query";
import type { PagedUserCarsResponse } from "@/generated/api/schemas";

const PAGE_SIZE = 24;

export const browseCarsInfiniteQueryOptions = (
  terms: Array<string>,
  sort: number,
) =>
  infiniteQueryOptions({
    queryKey: ["browse-cars", { terms, sort }] as const,
    queryFn: ({ pageParam }) =>
      searchCarsFn({
        data: {
          terms,
          sort,
          isPublic: true,
          page: pageParam,
          pageSize: PAGE_SIZE,
        },
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasMore ? allPages.length + 1 : undefined,
  });

export const browseCarsStatsQueryOptions = (terms: Array<string>) =>
  queryOptions({
    queryKey: ["browse-cars-stats", { terms }] as const,
    queryFn: () => getCarsBrowseStatsFn({ data: { terms, isPublic: true } }),
  });

export function sortKeyToNumber(
  key: CarSortKey | undefined,
  hasTerms: boolean,
): number {
  if (key === undefined)
    return hasTerms ? CAR_SEARCH_SORT.Relevance : CAR_SEARCH_SORT.Newest;
  return CAR_SEARCH_SORT[key];
}

function patchCarInPages(
  data: InfiniteData<PagedUserCarsResponse>,
  carId: number | string,
  patch: (
    car: PagedUserCarsResponse["cars"][number],
  ) => PagedUserCarsResponse["cars"][number],
): InfiniteData<PagedUserCarsResponse> {
  return {
    ...data,
    pages: data.pages.map((page) => ({
      ...page,
      cars: page.cars.map((car) => (car.id == carId ? patch(car) : car)),
    })),
  };
}

export function useToggleCarLike(carId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (isCurrentlyLiked: boolean) =>
      isCurrentlyLiked
        ? unlikeUserCarFn({ data: { carId } })
        : likeUserCarFn({ data: { carId } }),
    onMutate: async (isCurrentlyLiked) => {
      await queryClient.cancelQueries({ queryKey: ["browse-cars"] });
      const snapshots = queryClient.getQueriesData<
        InfiniteData<PagedUserCarsResponse>
      >({
        queryKey: ["browse-cars"],
      });
      snapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(
          key,
          patchCarInPages(data, carId, (car) => ({
            ...car,
            isLiked: !isCurrentlyLiked,
            likeCount: Number(car.likeCount) + (isCurrentlyLiked ? -1 : 1),
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
      queryClient.invalidateQueries({ queryKey: ["browse-cars"] });
    },
  });
}

export function useToggleCarSubscription(carId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (isCurrentlySubscribed: boolean) =>
      isCurrentlySubscribed
        ? unsubscribeFromUserCarFn({ data: { carId } })
        : subscribeToUserCarFn({ data: { carId } }),
    onMutate: async (isCurrentlySubscribed) => {
      await queryClient.cancelQueries({ queryKey: ["browse-cars"] });
      const snapshots = queryClient.getQueriesData<
        InfiniteData<PagedUserCarsResponse>
      >({
        queryKey: ["browse-cars"],
      });
      snapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(
          key,
          patchCarInPages(data, carId, (car) => ({
            ...car,
            isSubscribed: !isCurrentlySubscribed,
            subscriberCount:
              Number(car.subscriberCount) + (isCurrentlySubscribed ? -1 : 1),
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
      queryClient.invalidateQueries({ queryKey: ["browse-cars"] });
    },
  });
}
