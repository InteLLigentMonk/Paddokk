import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/profile')({
  component: ProfilePage,
})

function ProfilePage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Profile</Title>
        <Text c="dimmed">View and edit your public profile</Text>
      </Stack>
    </Container>
  )
}
