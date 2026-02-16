import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/journeys')({
  component: JourneysPage,
})

function JourneysPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>My Journeys</Title>
        <Text c="dimmed">Manage your car journeys and build logs</Text>
      </Stack>
    </Container>
  )
}
