import { createFileRoute, Link } from "@tanstack/react-router";
import { Container, Stack, Title, Text, Button, Group } from "@mantine/core";
import { ArrowLeft } from "lucide-react";

export const Route = createFileRoute("/_app/me/journeys/$journeyId/")({
  component: JourneyDetailPage,
});

function JourneyDetailPage() {
  const { journeyId } = Route.useParams();

  return (
    <Container size="lg" py="xl">
      <Stack gap="lg">
        <Group>
          <Button
            component={Link}
            to="/me/journeys"
            variant="subtle"
            leftSection={<ArrowLeft size={16} />}
          >
            Tillbaka till mina resor
          </Button>
        </Group>

        <Stack gap="xs">
          <Title order={1}>Resa #{journeyId}</Title>
          <Text c="dimmed">Detaljvyn för resan kommer snart.</Text>
        </Stack>
      </Stack>
    </Container>
  );
}
