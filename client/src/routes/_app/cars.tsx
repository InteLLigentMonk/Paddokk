import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/cars')({
  component: CarsPage,
})

function CarsPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>My Cars</Title>
        <Text c="dimmed">Manage your car collection and specifications</Text>
      </Stack>
    </Container>
  )
}
