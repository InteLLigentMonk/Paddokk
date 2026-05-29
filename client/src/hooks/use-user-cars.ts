import { useInfiniteQuery } from "@tanstack/react-query";
import { getUserCarsFn } from "@/lib/api/user-cars";
import { carKeys } from "@/lib/api/cars.keys";

const USER_CARS_PAGE_SIZE = 20;

export function useUserCarsInfinite(enabled = true) {
  return useInfiniteQuery({
    queryKey: carKeys.userCarsRoot,
    queryFn: ({ pageParam = 1 }) =>
      getUserCarsFn({
        data: { page: pageParam, pageSize: USER_CARS_PAGE_SIZE },
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
    enabled,
  });
}
