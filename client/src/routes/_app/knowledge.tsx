import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/knowledge')({
  component: KnowledgePage,
})

function KnowledgePage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Knowledge Base</Title>
        <Text c="dimmed">Learn about car maintenance, modifications, and best practices</Text>
      </Stack>
    </Container>
  )
}
