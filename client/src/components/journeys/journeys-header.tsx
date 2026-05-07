import { Group, Title, Text, Button, Stack } from "@mantine/core";
import { Plus, TrendingUp } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";
import { useCanAddJourney } from "@/hooks/use-can-add-journey";

export function JourneysHeader() {
  const navigate = useNavigate();
  const { canAdd, isLoading } = useCanAddJourney();

  const handleNewJourney = () => {
    if (canAdd) {
      openCreateJourneyModal();
    } else {
      navigate({ to: "/me/subscription", search: {} });
    }
  };

  return (
    <Group justify="space-between" align="flex-start" wrap="wrap">
      <Stack gap={4}>
        <Title order={2}>My Journeys</Title>
        <Text c="dimmed" size="sm">
          Document and share your car building journeys
        </Text>
      </Stack>

      <Button
        leftSection={
          canAdd || isLoading ? <Plus size={18} /> : <TrendingUp size={18} />
        }
        onClick={handleNewJourney}
        loading={isLoading}
        style={{ minWidth: 140 }}
        visibleFrom="sm"
      >
        {canAdd || isLoading ? "New Journey" : "Upgrade"}
      </Button>
    </Group>
  );
}
