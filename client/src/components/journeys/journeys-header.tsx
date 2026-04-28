import { Group, Title, Text, Button, Stack } from "@mantine/core";
import { Plus } from "lucide-react";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";

export function JourneysHeader() {
  return (
    <Group
      justify="space-between"
      align="flex-start"
      mb="xl"
      wrap="wrap"
      gap="md"
    >
      <Stack gap={4}>
        <Title order={2}>My Journeys</Title>
        <Text c="dimmed" size="sm">
          Document and share your car building journeys
        </Text>
      </Stack>

      <Button
        leftSection={<Plus size={18} />}
        onClick={openCreateJourneyModal}
        style={{ minWidth: 140 }}
      >
        Ny resa
      </Button>
    </Group>
  );
}
