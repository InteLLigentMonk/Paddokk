import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Alert } from "@mantine/core";
import { AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import {
  getUserJourneysFn,
  getDefaultActiveJourneyFn,
} from "@/lib/api/user-journeys.server";
import { JourneysHeader } from "@/components/journeys/journeys-header";
import { JourneysGrid } from "@/components/journeys/journeys-grid";
import { CreateJourneyModal } from "@/components/journeys/create-journey-modal";
import { EditJourneyModal } from "@/components/journeys/edit-journey-modal";
import { DeleteJourneyConfirm } from "@/components/journeys/delete-journey-confirm";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";

export const Route = createFileRoute("/_app/me/journeys/")({
  loader: ({ context: { queryClient } }) =>
    queryClient.ensureQueryData({
      queryKey: ["user-journeys"],
      queryFn: () => getUserJourneysFn(),
    }),
  component: JourneysPage,
});

function JourneysPage() {
  const { data: journeys, isLoading, error } = useQuery({
    queryKey: ["user-journeys"],
    queryFn: () => getUserJourneysFn(),
  });

  const { data: defaultJourney } = useQuery({
    queryKey: ["default-active-journey"],
    queryFn: () => getDefaultActiveJourneyFn(),
  });

  const journeyList = journeys ?? [];
  const defaultJourneyId = defaultJourney ? Number(defaultJourney.id) : null;

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <JourneysHeader />

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Fel" color="red">
            Kunde inte ladda resor. Försök igen.
          </Alert>
        ) : (
          <JourneysGrid
            journeys={journeyList}
            defaultJourneyId={defaultJourneyId}
            isLoading={isLoading}
            onCreateJourney={openCreateJourneyModal}
          />
        )}
      </Stack>

      <CreateJourneyModal />
      <EditJourneyModal />
      <DeleteJourneyConfirm />
    </Container>
  );
}
