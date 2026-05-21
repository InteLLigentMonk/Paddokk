import { useEffect } from "react";
import { Box, SimpleGrid, Skeleton, Stack } from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { useInfiniteQuery } from "@tanstack/react-query";
import { JourneyBrowseCard } from "./journey-browse-card";
import { JourneysBrowseEmptyState } from "./journeys-browse-empty-state";
import { browseJourneysInfiniteQueryOptions } from "@/lib/api/journeys.queries";

interface JourneysBrowseGridProps {
  terms: Array<string>;
  sort: number;
}

function CardSkeleton() {
  return (
    <Stack gap="xs">
      <Skeleton height={180} radius="md" />
      <Skeleton height={16} width="70%" />
      <Skeleton height={12} width="40%" />
      <Skeleton height={12} width="55%" />
    </Stack>
  );
}

export function JourneysBrowseGrid({ terms, sort }: JourneysBrowseGridProps) {
  const { ref, entry } = useIntersection({ root: null, threshold: 0.1 });

  const { data, isLoading, isFetchingNextPage, fetchNextPage, hasNextPage } =
    useInfiniteQuery(browseJourneysInfiniteQueryOptions(terms, sort));

  useEffect(() => {
    if (entry?.isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [entry?.isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const journeys = data?.pages.flatMap((p) => p.journeys) ?? [];

  if (isLoading) {
    return (
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md">
        {Array.from({ length: 8 }).map((_, i) => (
          <CardSkeleton key={i} />
        ))}
      </SimpleGrid>
    );
  }

  if (journeys.length === 0) {
    return <JourneysBrowseEmptyState />;
  }

  return (
    <>
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md">
        {journeys.map((journey) => (
          <JourneyBrowseCard key={String(journey.id)} journey={journey} />
        ))}
        {isFetchingNextPage &&
          Array.from({ length: 4 }).map((_, i) => (
            <CardSkeleton key={`loading-${i}`} />
          ))}
      </SimpleGrid>
      <Box ref={ref} h={1} aria-hidden />
    </>
  );
}
