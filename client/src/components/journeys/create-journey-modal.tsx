import { Modal, Stack, Text } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import {
  journeysPageStore,
  closeCreateJourneyModal,
} from "@/lib/stores/journeys-page-store";

export function CreateJourneyModal() {
  const isOpen = useStore(
    journeysPageStore,
    (state) => state.modals.createJourney,
  );

  return (
    <Modal
      opened={isOpen}
      onClose={closeCreateJourneyModal}
      title="Ny resa"
      centered
      size="lg"
    >
      <Stack gap="md">
        <Text c="dimmed">
          Formulär för att skapa en ny resa kommer snart. Här kommer du kunna
          välja bil, kategori, måldatum och skriva ditt första inlägg med
          bilder.
        </Text>
      </Stack>
    </Modal>
  );
}
