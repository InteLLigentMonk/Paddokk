import { Button, Center, Stack, Text } from "@mantine/core";
import { Compass } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";

export function JourneysBrowseEmptyState() {
  const navigate = useNavigate();

  return (
    <Center py="xl">
      <Stack align="center" gap="md">
        <Compass size={48} strokeWidth={1.2} color="var(--mantine-color-dimmed)" />
        <Stack align="center" gap={4}>
          <Text fw={500}>Inga journeys matchade</Text>
          <Text size="sm" c="dimmed">
            Prova att rensa filtret eller söka på något annat.
          </Text>
        </Stack>
        <Button
          variant="light"
          size="sm"
          onClick={() => navigate({ to: "/journeys", search: {} })}
        >
          Rensa filter
        </Button>
      </Stack>
    </Center>
  );
}
