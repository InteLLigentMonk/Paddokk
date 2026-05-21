import { Container, Stack, Text, Title } from "@mantine/core";
import { JourneysStatsCard } from "./journeys-stats-card";
import { JourneysFilterBar } from "./journeys-filter-bar";
import { JourneysBrowseGrid } from "./journeys-browse-grid";
import { sortKeyToNumber } from "@/lib/api/journeys.queries";
import { Route } from "@/routes/_app/journeys/index";

export function JourneysBrowsePage() {
  const { q, sort } = Route.useSearch();
  const terms = q ?? [];
  const sortKey = sort;
  const sortNum = sortKeyToNumber(sortKey, terms.length > 0);

  return (
    <Container size="xl" py="xl">
      <Stack gap="lg">
        <Stack gap="xs">
          <Title order={1}>Journeys</Title>
          <Text c="var(--mantine-color-gray-6)" fz="md" maw={600}>
            Following the cars and the people behind them — builds in progress,
            restorations, road trips, track days. Filter by anything: car, era,
            style, or stage.
          </Text>
        </Stack>
        <JourneysStatsCard terms={terms} />
        <JourneysFilterBar terms={terms} sort={sortKey} />
        <JourneysBrowseGrid terms={terms} sort={sortNum} />
      </Stack>
    </Container>
  );
}
