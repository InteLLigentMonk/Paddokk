import { createFileRoute } from '@tanstack/react-router'
import { Container, Title, Text, Stack } from '@mantine/core'

export const Route = createFileRoute('/_app/inventory')({
  component: InventoryPage,
})

function InventoryPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Inventory List</Title>
        <Text c="dimmed">Track parts and components for your builds — coming soon</Text>
      </Stack>
    </Container>
  )
}
