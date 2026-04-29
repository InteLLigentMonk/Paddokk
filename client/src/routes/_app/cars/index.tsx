import { createFileRoute } from "@tanstack/react-router"
import { Container, Stack, Title, Text } from "@mantine/core"

export const Route = createFileRoute("/_app/cars/")({
  component: CarsHubPage,
})

function CarsHubPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Cars</Title>
        <Text c="dimmed">Trending builds and updates from cars you follow — coming soon.</Text>
      </Stack>
    </Container>
  )
}
