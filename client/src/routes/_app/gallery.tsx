import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/gallery')({
  component: GalleryPage,
})

function GalleryPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Photo Gallery</Title>
        <Text c="dimmed">Browse and manage your car photos</Text>
      </Stack>
    </Container>
  )
}
