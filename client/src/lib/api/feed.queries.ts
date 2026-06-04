import { infiniteQueryOptions } from "@tanstack/react-query";
import { getFeedFn } from "./feed";
import { feedKeys } from "./feed.keys";
import { requirePage } from "./infinite";

const PAGE_SIZE = 20;

/**
 * Infinite, strictly chronological feed. Pages are DB-paginated server-side; the next
 * page is fetched only while the backend reports more remain (`hasNextPage`).
 */
export const feedInfiniteQueryOptions = () =>
  infiniteQueryOptions({
    queryKey: feedKeys.root,
    queryFn: ({ pageParam }) =>
      requirePage(
        getFeedFn({ data: { page: pageParam, pageSize: PAGE_SIZE } }),
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasNextPage ? allPages.length + 1 : undefined,
  });
