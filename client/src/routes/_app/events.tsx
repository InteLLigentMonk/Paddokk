import { createFileRoute } from '@tanstack/react-router'
import { Container, Stack, Text, Title } from '@mantine/core'

export const Route = createFileRoute('/_app/events')({
  component: EventsPage,
})

function EventsPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Events</Title>
        <Text c="dimmed">Upcoming meets, shows, and track days — coming soon</Text>
      </Stack>
    </Container>
  )
}
