import {
  type InfiniteData,
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { notifications as toast } from "@mantine/notifications";
import type { PagedResultOfNotificationDto } from "@/generated/api/schemas";
import {
  getNotificationsFn,
  getUnreadCountFn,
  markAllReadFn,
  markNotificationReadFn,
} from "./notifications";
import { notificationKeys } from "./notifications.keys";
import { requirePage } from "./infinite";

const PAGE_SIZE = 20;

/** Slow background poll for the bell badge, plus a refetch whenever the window regains focus. */
export const unreadCountQueryOptions = () =>
  queryOptions({
    queryKey: notificationKeys.unreadCount,
    queryFn: () => getUnreadCountFn(),
    refetchInterval: 60_000,
    refetchOnWindowFocus: true,
  });

/**
 * Paginated notifications, newest first. `unread` narrows server-side:
 * undefined = all, true = unread only, false = read only.
 */
export const notificationsInfiniteQueryOptions = (unread?: boolean) =>
  infiniteQueryOptions({
    queryKey: notificationKeys.list(unread),
    queryFn: ({ pageParam }) =>
      requirePage(
        getNotificationsFn({
          data: { unread, page: pageParam, pageSize: PAGE_SIZE },
        }),
      ),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasNextPage ? allPages.length + 1 : undefined,
  });

type NotificationListData = InfiniteData<PagedResultOfNotificationDto>;

const listQueryFilter = { queryKey: ["notifications", "list"] as const };

/**
 * Optimistically flips one row to read and decrements the badge (only if it was unread), so the
 * UI responds instantly on click. Rolls back on error and reconciles with the server on settle.
 */
export function useMarkNotificationRead() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => markNotificationReadFn({ data: { id } }),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: notificationKeys.root });

      const listSnapshots =
        queryClient.getQueriesData<NotificationListData>(listQueryFilter);
      const countSnapshot = queryClient.getQueryData<number>(
        notificationKeys.unreadCount,
      );

      let flippedUnread = false;
      listSnapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData<NotificationListData>(key, {
          ...data,
          pages: data.pages.map((page) => ({
            ...page,
            items: page.items.map((item) => {
              const isTargetUnread = item.id === id && !item.read;
              if (isTargetUnread) {
                flippedUnread = true;
              }
              return isTargetUnread ? { ...item, read: true } : item;
            }),
          })),
        });
      });

      if (flippedUnread && countSnapshot != null) {
        queryClient.setQueryData(
          notificationKeys.unreadCount,
          Math.max(0, countSnapshot - 1),
        );
      }

      return { listSnapshots, countSnapshot };
    },
    onError: (_err, _id, context) => {
      context?.listSnapshots.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      if (context?.countSnapshot !== undefined) {
        queryClient.setQueryData(
          notificationKeys.unreadCount,
          context.countSnapshot,
        );
      }
      toast.show({
        color: "red",
        message: "Could not mark notification as read. Please try again.",
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: notificationKeys.root });
    },
  });
}

/** Optimistically marks every row read and zeroes the badge. */
export function useMarkAllRead() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => markAllReadFn(),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: notificationKeys.root });

      const listSnapshots =
        queryClient.getQueriesData<NotificationListData>(listQueryFilter);
      const countSnapshot = queryClient.getQueryData<number>(
        notificationKeys.unreadCount,
      );

      listSnapshots.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData<NotificationListData>(key, {
          ...data,
          pages: data.pages.map((page) => ({
            ...page,
            items: page.items.map((item) => ({ ...item, read: true })),
          })),
        });
      });

      queryClient.setQueryData(notificationKeys.unreadCount, 0);

      return { listSnapshots, countSnapshot };
    },
    onError: (_err, _vars, context) => {
      context?.listSnapshots.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      if (context?.countSnapshot !== undefined) {
        queryClient.setQueryData(
          notificationKeys.unreadCount,
          context.countSnapshot,
        );
      }
      toast.show({
        color: "red",
        message: "Could not mark all as read. Please try again.",
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: notificationKeys.root });
    },
  });
}
