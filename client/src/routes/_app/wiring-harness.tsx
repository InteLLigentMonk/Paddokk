import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/wiring-harness')({
  component: WiringHarnessPage,
})

function WiringHarnessPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Wiring Harness Creator</Title>
        <Text c="dimmed">Design and document your wiring harness — coming soon</Text>
      </Stack>
    </Container>
  )
}
