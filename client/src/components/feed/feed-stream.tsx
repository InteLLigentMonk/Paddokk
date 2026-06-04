import { Fragment, useEffect } from "react";
import {
  Alert,
  Box,
  Center,
  Loader,
  Skeleton,
  Stack,
  Text,
} from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { useInfiniteQuery } from "@tanstack/react-query";
import { AlertCircle } from "lucide-react";
import { FeedDayDivider } from "./feed-day-divider";
import { FeedEmptyState } from "./feed-empty-state";
import { FeedJourneyPostCard } from "./feed-journey-post-card";
import { FeedLifecycleCard } from "./feed-lifecycle-card";
import { FEED_ITEM_TYPE, feedItemKey } from "./feed-item-type";
import type { FeedItemDto } from "@/generated/api/schemas";
import { dayKey } from "@/lib/feed/feed-time";
import { feedInfiniteQueryOptions } from "@/lib/api/feed.queries";

function FeedCardSkeleton() {
  return (
    <Stack gap="xs">
      <Skeleton height={48} radius="md" />
      <Skeleton height={180} radius="md" />
      <Skeleton height={12} width="60%" />
    </Stack>
  );
}

/**
 * Renders a single feed item by its discriminator. PhotosAdded and SpecChanged land in
 * later slices; until then they fall through to nothing (the union type stays closed, so
 * this switch stays exhaustive).
 */
function FeedItem({ item }: { item: FeedItemDto }) {
  switch (item.type) {
    case FEED_ITEM_TYPE.JourneyPost:
      return <FeedJourneyPostCard item={item} />;
    case FEED_ITEM_TYPE.UserCarCreated:
    case FEED_ITEM_TYPE.JourneyStarted:
    case FEED_ITEM_TYPE.JourneyCompleted:
      return <FeedLifecycleCard item={item} />;
    default:
      return null;
  }
}

/**
 * The strictly chronological feed column. Flattens the DB-paginated pages into one
 * stream, segments it by day with dividers (ADR-0006), and fetches the next page
 * when a sentinel at the bottom scrolls into view.
 */
export function FeedStream() {
  const { ref, entry } = useIntersection({ root: null, threshold: 0.1 });

  const {
    data,
    isLoading,
    isError,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
  } = useInfiniteQuery(feedInfiniteQueryOptions());

  useEffect(() => {
    if (entry?.isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [entry?.isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  if (isLoading) {
    return (
      <Stack gap="lg" aria-busy="true">
        {Array.from({ length: 3 }).map((_, i) => (
          <FeedCardSkeleton key={i} />
        ))}
      </Stack>
    );
  }

  if (isError) {
    return (
      <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
        Could not load your feed. Please try again.
      </Alert>
    );
  }

  const items = data?.pages.flatMap((page) => page.items) ?? [];

  if (items.length === 0) {
    return <FeedEmptyState />;
  }

  let lastDayKey: string | null = null;

  return (
    <Stack gap="lg">
      {items.map((item) => {
        const itemDayKey = dayKey(item.createdAt);
        const showDivider = itemDayKey !== lastDayKey;
        lastDayKey = itemDayKey;
        return (
          <Fragment key={feedItemKey(item)}>
            {showDivider && <FeedDayDivider iso={item.createdAt} />}
            <FeedItem item={item} />
          </Fragment>
        );
      })}

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
