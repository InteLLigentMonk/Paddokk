import { Stack, Text, Button, Box } from "@mantine/core";
import { BookOpen } from "lucide-react";

interface JourneysEmptyStateProps {
  onCreateJourney: () => void;
}

export function JourneysEmptyState({
  onCreateJourney,
}: JourneysEmptyStateProps) {
  return (
    <Stack
      align="center"
      justify="center"
      gap="xl"
      py="xl"
      style={{ minHeight: 300 }}
    >
      <Box component="span" darkHidden>
        <BookOpen size={64} style={{ color: "var(--mantine-color-gray-4)" }} />
      </Box>
      <Box component="span" lightHidden>
        <BookOpen size={64} style={{ color: "var(--mantine-color-dark-4)" }} />
      </Box>

      <Stack align="center" gap="xs">
        <Text size="lg" fw={500}>
          Inga resor ännu
        </Text>
        <Text size="sm" c="dimmed" ta="center">
          Skapa din första resa för att dokumentera din bilbyggar-resa
        </Text>
      </Stack>

      <Button onClick={onCreateJourney}>Skapa din första resa</Button>
    </Stack>
  );
}
