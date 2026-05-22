import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Text, Title } from "@mantine/core";

export const Route = createFileRoute("/_app/community")({
  component: CommunityPage,
});

function CommunityPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Community</Title>
        <Text c="dimmed">Connect with other car enthusiasts</Text>
      </Stack>
    </Container>
  );
}
