import { createFileRoute } from "@tanstack/react-router";
import { Button, Container, Stack, Title } from "@mantine/core";
import { useNotifications } from "@/integrations/mantine";

export const Route = createFileRoute("/")({ component: App });

function App() {
  const notifications = useNotifications();

  return (
    <Container py="xl">
      <Stack gap="md">
        <Title order={1}>Welcome to Paddokk</Title>
        <Button
          onClick={() =>
            notifications.success({
              message: "Notification system is working!",
            })
          }
        >
          Test Notification
        </Button>
        <Button
          variant="outline"
          color="red"
          onClick={() =>
            notifications.error({
              message: "Notification system is working!",
            })
          }
        >
          Test Notification
        </Button>
      </Stack>
    </Container>
  );
}
