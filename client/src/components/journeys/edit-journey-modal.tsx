import { Modal, Stack, Text } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import {
  journeysPageStore,
  closeEditJourneyModal,
} from "@/lib/stores/journeys-page-store";

export function EditJourneyModal() {
  const editState = useStore(
    journeysPageStore,
    (state) => state.modals.editJourney,
  );

  return (
    <Modal
      opened={editState.open}
      onClose={closeEditJourneyModal}
      title="Redigera resa"
      centered
      size="lg"
    >
      <Stack gap="md">
        <Text c="dimmed">
          Redigering av resans titel, beskrivning, kategori och måldatum kommer
          snart.
        </Text>
      </Stack>
    </Modal>
  );
}
