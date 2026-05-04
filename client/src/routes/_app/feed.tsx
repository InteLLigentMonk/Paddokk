import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/feed')({
  component: FeedPage,
})

function FeedPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Feed</Title>
        <Text c="dimmed">Your personalized feed — coming soon</Text>
      </Stack>
    </Container>
  )
}
