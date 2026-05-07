import { createFileRoute } from "@tanstack/react-router";
import { Container, Title, Text, Stack } from "@mantine/core";

export const Route = createFileRoute("/_app/me/notifications")({
  component: NotificationsPage,
});

function NotificationsPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Notifications</Title>
        <Text c="dimmed">Stay updated with your activity and mentions</Text>
      </Stack>
    </Container>
  );
}
