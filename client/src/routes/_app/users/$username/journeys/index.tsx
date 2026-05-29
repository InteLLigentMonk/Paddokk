import { createFileRoute, notFound } from "@tanstack/react-router";
import { Alert, Container, Stack, Text, Title } from "@mantine/core";
import { AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { JourneysHeader } from "@/components/journeys/journeys-header";
import { JourneysGrid } from "@/components/journeys/journeys-grid";
import { EditJourneyModal } from "@/components/journeys/edit-journey-modal";
import { DeleteJourneyConfirm } from "@/components/journeys/delete-journey-confirm";
import {
  userByUsernameQueryOptions,
  userJourneysByUsernameQueryOptions,
} from "@/lib/api/users.queries";
import { getDefaultActiveJourneyFn } from "@/lib/api/user-journeys";
import { journeyKeys } from "@/lib/api/journeys.keys";
import { useCurrentUser } from "@/hooks/use-current-user";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";

export const Route = createFileRoute("/_app/users/$username/journeys/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await queryClient.ensureQueryData(
        userByUsernameQueryOptions(params.username),
      );
    } catch {
      throw notFound();
    }
    await queryClient.ensureQueryData(
      userJourneysByUsernameQueryOptions(params.username),
    );
  },
  component: UserJourneysPage,
});

function UserJourneysPage() {
  const { username } = Route.useParams();
  const { data: currentUser } = useCurrentUser();
  const isOwner = currentUser?.username === username;

  const {
    data: journeys,
    isLoading,
    error,
  } = useQuery(userJourneysByUsernameQueryOptions(username));

  const { data: defaultJourney } = useQuery({
    queryKey: journeyKeys.defaultActiveJourney,
    queryFn: () => getDefaultActiveJourneyFn(),
    enabled: isOwner,
  });

  const defaultJourneyId =
    isOwner && defaultJourney ? Number(defaultJourney.id) : null;

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        {isOwner ? (
          <JourneysHeader />
        ) : (
          <Stack gap={4}>
            <Title order={2}>@{username} â€” Journeys</Title>
            <Text c="dimmed" size="sm">
              Journeys shared by @{username}
            </Text>
          </Stack>
        )}

        {error ? (
          <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
            Failed to load journeys. Please try again.
          </Alert>
        ) : (
          <JourneysGrid
            journeys={journeys ?? []}
            defaultJourneyId={defaultJourneyId}
            isLoading={isLoading}
            onCreateJourney={openCreateJourneyModal}
          />
        )}
      </Stack>

      {isOwner && (
        <>
          <EditJourneyModal />
          <DeleteJourneyConfirm />
        </>
      )}
    </Container>
  );
}
