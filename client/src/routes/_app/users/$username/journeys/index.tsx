import { createFileRoute, notFound } from "@tanstack/react-router";
import { Container, Stack, Title, Alert } from "@mantine/core";
import { AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { JourneysGrid } from "@/components/journeys/journeys-grid";
import {
  getUserByUsernameFn,
  getUserJourneysByUsernameFn,
} from "@/lib/api/users.server";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";

export const Route = createFileRoute("/_app/users/$username/journeys/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await getUserByUsernameFn({ data: { username: params.username } });
    } catch {
      throw notFound();
    }
    await queryClient.ensureQueryData({
      queryKey: ["user-journeys-by-username", params.username],
      queryFn: () =>
        getUserJourneysByUsernameFn({ data: { username: params.username } }),
    });
  },
  component: UserJourneysPage,
});

function UserJourneysPage() {
  const { username } = Route.useParams();
  const { data: journeys, isLoading, error } = useQuery({
    queryKey: ["user-journeys-by-username", username],
    queryFn: () =>
      getUserJourneysByUsernameFn({ data: { username } }),
  });

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <Title order={2}>@{username} — Journeys</Title>

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Fel" color="red">
            Kunde inte ladda resor. Försök igen.
          </Alert>
        ) : (
          <JourneysGrid
            journeys={journeys ?? []}
            isLoading={isLoading}
            onCreateJourney={openCreateJourneyModal}
          />
        )}
      </Stack>
    </Container>
  );
}
