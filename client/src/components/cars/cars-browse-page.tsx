import { Container, Stack, Title } from "@mantine/core";
import { Route } from "@/routes/_app/cars/index";
import { sortKeyToNumber } from "@/lib/api/cars.queries";
import type { CarSortKey } from "@/lib/api/cars.server";
import { CarsStatsCard } from "./cars-stats-card";
import { CarsFilterBar } from "./cars-filter-bar";
import { CarsBrowseGrid } from "./cars-browse-grid";

export function CarsBrowsePage() {
  const { q, sort } = Route.useSearch();
  const terms = q ?? [];
  const sortKey = sort as CarSortKey | undefined;
  const sortNum = sortKeyToNumber(sortKey, terms.length > 0);

  return (
    <Container size="xl" py="xl">
      <Stack gap="lg">
        <Title order={1}>Bilar</Title>
        <CarsStatsCard terms={terms} />
        <CarsFilterBar terms={terms} sort={sortKey} />
        <CarsBrowseGrid terms={terms} sort={sortNum} />
      </Stack>
    </Container>
  );
}
