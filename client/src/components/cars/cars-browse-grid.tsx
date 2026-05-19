import { useEffect } from "react";
import { Box, SimpleGrid, Skeleton, Stack } from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { useInfiniteQuery } from "@tanstack/react-query";
import { browseCarsInfiniteQueryOptions } from "@/lib/api/cars.queries";
import { CarBrowseCard } from "./car-browse-card";
import { CarsBrowseEmptyState } from "./cars-browse-empty-state";

interface CarsBrowseGridProps {
  terms: string[];
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

export function CarsBrowseGrid({ terms, sort }: CarsBrowseGridProps) {
  const { ref, entry } = useIntersection({ root: null, threshold: 0.1 });

  const {
    data,
    isLoading,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
  } = useInfiniteQuery(browseCarsInfiniteQueryOptions(terms, sort));

  useEffect(() => {
    if (entry?.isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [entry?.isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const cars = data?.pages.flatMap((p) => p.cars) ?? [];

  if (isLoading) {
    return (
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3, lg: 4 }} spacing="md">
        {Array.from({ length: 8 }).map((_, i) => (
          <CardSkeleton key={i} />
        ))}
      </SimpleGrid>
    );
  }

  if (cars.length === 0) {
    return <CarsBrowseEmptyState />;
  }

  return (
    <>
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3, lg: 4 }} spacing="md">
        {cars.map((car) => (
          <CarBrowseCard key={String(car.id)} car={car} />
        ))}
        {isFetchingNextPage &&
          Array.from({ length: 4 }).map((_, i) => <CardSkeleton key={`loading-${i}`} />)}
      </SimpleGrid>
      <Box ref={ref} h={1} aria-hidden />
    </>
  );
}
