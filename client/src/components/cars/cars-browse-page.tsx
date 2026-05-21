import { Container, Stack, Title, Text } from "@mantine/core";
import { Route } from "@/routes/_app/cars/index";
import { sortKeyToNumber } from "@/lib/api/cars.queries";
import type { CarSortKey } from "@/lib/api/cars";
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
        <Stack gap="xs">
          <Title order={1}>Cars</Title>
          <Text c="var(--mantine-color-gray-6)" fz="md" maw={600}>
            Every car the community has documented â€” from air-cooled 911s and
            air-conditioned daily drivers to time-attack builds. Filter by
            anything: make, era, region, engine, owner.
          </Text>
        </Stack>
        <CarsStatsCard terms={terms} />
        <CarsFilterBar terms={terms} sort={sortKey} />
        <CarsBrowseGrid terms={terms} sort={sortNum} />
      </Stack>
    </Container>
  );
}
