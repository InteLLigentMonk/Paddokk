import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Text, Title } from "@mantine/core";

export const Route = createFileRoute("/_app/me/notifications")({
  staticData: { breadcrumb: "Notifications" },
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
