import { SimpleGrid, Skeleton } from "@mantine/core";
import { JourneyCard } from "./journey-card";
import { JourneysEmptyState } from "./journeys-empty-state";
import type { JourneyDto } from "@/generated/api/schemas";

interface JourneysGridProps {
  journeys: Array<JourneyDto>;
  defaultJourneyId?: number | null;
  isLoading?: boolean;
  onCreateJourney: () => void;
}

export function JourneysGrid({
  journeys,
  defaultJourneyId,
  isLoading,
  onCreateJourney,
}: JourneysGridProps) {
  if (isLoading) {
    return (
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
        {Array.from({ length: 6 }).map((_, i) => (
          <Skeleton key={i} height={400} radius="md" />
        ))}
      </SimpleGrid>
    );
  }

  if (journeys.length === 0) {
    return <JourneysEmptyState onCreateJourney={onCreateJourney} />;
  }

  return (
    <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="lg">
      {journeys.map((journey) => (
        <JourneyCard
          key={journey.id}
          journey={journey}
          isDefault={Number(journey.id) === defaultJourneyId}
        />
      ))}
    </SimpleGrid>
  );
}
