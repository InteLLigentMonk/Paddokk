import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Text, Title } from "@mantine/core";

export const Route = createFileRoute("/_app/marketplace")({
  component: MarketplacePage,
});

function MarketplacePage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Marketplace</Title>
        <Text c="dimmed">
          Buy and sell parts, cars, and accessories — coming soon
        </Text>
      </Stack>
    </Container>
  );
}
