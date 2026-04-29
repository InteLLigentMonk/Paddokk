import { createFileRoute } from "@tanstack/react-router"
import { Container, Stack, Title, Text } from "@mantine/core"

export const Route = createFileRoute("/_app/journeys/")({
  component: JourneysHubPage,
})

function JourneysHubPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Journeys</Title>
        <Text c="dimmed">Trending journeys and updates from people you follow — coming soon.</Text>
      </Stack>
    </Container>
  )
}
