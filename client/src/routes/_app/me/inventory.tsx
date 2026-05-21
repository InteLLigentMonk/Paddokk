import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Text, Title } from "@mantine/core";

export const Route = createFileRoute("/_app/me/inventory")({
  staticData: { breadcrumb: "Inventory" },
  component: InventoryPage,
});

function InventoryPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={2}>Inventory List</Title>
        <Text c="dimmed">
          Track parts and components for your builds — coming soon
        </Text>
      </Stack>
    </Container>
  );
}
