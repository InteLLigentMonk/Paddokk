import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/explore')({
  component: ExplorePage,
})

function ExplorePage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Explore</Title>
        <Text c="dimmed">Discover public journeys and car builds from the community</Text>
      </Stack>
    </Container>
  )
}
