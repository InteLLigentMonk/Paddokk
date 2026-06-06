import { useEffect } from "react";
import {
  Alert,
  Box,
  Center,
  Loader,
  Paper,
  Skeleton,
  Stack,
  Text,
} from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { useInfiniteQuery } from "@tanstack/react-query";
import { AlertCircle, BellOff } from "lucide-react";
import { NotificationItem } from "./notification-item";
import { notificationsInfiniteQueryOptions } from "@/lib/api/notifications.queries";
import { useNotificationActivate } from "@/lib/notifications/use-notification-activate";

function NotificationListSkeleton() {
  return (
    <Stack gap="xs" aria-busy="true">
      {Array.from({ length: 5 }).map((_, i) => (
        <Skeleton key={i} height={64} radius="md" />
      ))}
    </Stack>
  );
}

function NotificationsEmptyState() {
  return (
    <Paper withBorder p="xl" radius="md">
      <Stack align="center" gap="xs">
        <BellOff size={32} strokeWidth={1.5} color="var(--mantine-color-dimmed)" />
        <Text fw={600}>No notifications yet</Text>
        <Text size="sm" c="dimmed" ta="center">
          When someone interacts with your journeys, cars, or comments, it shows up here.
        </Text>
      </Stack>
    </Paper>
  );
}

/**
 * The full notification history for the hub. Flattens the DB-paginated pages into one list and
 * fetches the next page when a bottom sentinel scrolls into view (same shape as the feed stream).
 */
export function NotificationList() {
  const activate = useNotificationActivate();
  const { ref, entry } = useIntersection({ root: null, threshold: 0.1 });

  const {
    data,
    isLoading,
    isError,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
  } = useInfiniteQuery(notificationsInfiniteQueryOptions());

  useEffect(() => {
    if (entry?.isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [entry?.isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  if (isLoading) {
    return <NotificationListSkeleton />;
  }

  if (isError) {
    return (
      <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
        Could not load your notifications. Please try again.
      </Alert>
    );
  }

  const items = data?.pages.flatMap((page) => page.items) ?? [];

  if (items.length === 0) {
    return <NotificationsEmptyState />;
  }

  return (
    <Stack gap={4}>
      {items.map((notification) => (
        <NotificationItem
          key={notification.id}
          notification={notification}
          onActivate={activate}
        />
      ))}

      <Box ref={ref} h={1} aria-hidden />

      {isFetchingNextPage && (
        <Center py="md" aria-busy="true">
          <Loader size="sm" />
        </Center>
      )}

      {!hasNextPage && (
        <Text size="xs" c="dimmed" ta="center" py="md">
          You're all caught up.
        </Text>
      )}
    </Stack>
  );
}
