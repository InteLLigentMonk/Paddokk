import { createFileRoute } from "@tanstack/react-router";
import { Container, Title, Text, Stack } from "@mantine/core";

export const Route = createFileRoute("/_app/me/settings")({
  staticData: { breadcrumb: "Settings" },
  component: SettingsPage,
});

function SettingsPage() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Title order={1}>Settings</Title>
        <Text c="dimmed">Manage your account preferences and privacy</Text>
      </Stack>
    </Container>
  );
}
