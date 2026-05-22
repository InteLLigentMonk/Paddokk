import { Box, Button, Stack, Text } from "@mantine/core";
import { Car } from "lucide-react";

interface CarsEmptyStateProps {
  onAddCar: () => void;
}

export function CarsEmptyState({ onAddCar }: CarsEmptyStateProps) {
  return (
    <Stack
      align="center"
      justify="center"
      gap="xl"
      py="xl"
      style={{ minHeight: 300 }}
    >
      <Box component="span" darkHidden>
        <Car size={64} style={{ color: "var(--mantine-color-gray-4)" }} />
      </Box>
      <Box component="span" lightHidden>
        <Car size={64} style={{ color: "var(--mantine-color-dark-4)" }} />
      </Box>

      <Stack align="center" gap="xs">
        <Text size="lg" fw={500}>
          No cars yet
        </Text>
        <Text size="sm" c="dimmed" ta="center">
          Add your first car to start tracking your journeys
        </Text>
      </Stack>

      <Button onClick={onAddCar}>Add Your First Car</Button>
    </Stack>
  );
}
