import { Modal, Text, Button, Group, Stack } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  journeysPageStore,
  closeDeleteJourneyConfirm,
} from "@/lib/stores/journeys-page-store";
import { userJourneysGetUserJourney } from "@/generated/api/user-journeys/user-journeys";
import { deleteUserJourneyFn, getDefaultActiveJourneyFn } from "@/lib/api/user-journeys.server";
import { useNotifications } from "@/integrations/mantine";

export function DeleteJourneyConfirm() {
  const deleteState = useStore(
    journeysPageStore,
    (state) => state.modals.deleteJourney,
  );
  const { open: isOpen, journeyId } = deleteState;
  const queryClient = useQueryClient();
  const notifications = useNotifications();

  const { data } = useQuery({
    queryKey: ["user-journey", journeyId],
    queryFn: () => userJourneysGetUserJourney(journeyId!),
    enabled: isOpen && !!journeyId,
  });

  const { data: defaultJourney } = useQuery({
    queryKey: ["default-active-journey"],
    queryFn: () => getDefaultActiveJourneyFn(),
    enabled: isOpen,
  });

  const journey = data?.status === 200 ? data.data : undefined;
  const isDefault = defaultJourney ? Number(defaultJourney.id) === journeyId : false;

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteUserJourneyFn({ data: { journeyId: id } }),
    onError: () => {
      notifications.error({
        message: "Kunde inte ta bort resan. Försök igen.",
      });
    },
    onSuccess: () => {
      notifications.success({
        message: isDefault ? "Resan borttagen, aktiv resa uppdaterad" : "Resan borttagen",
      });
      queryClient.invalidateQueries({
        predicate: (q) => {
          const key = q.queryKey[0];
          return key === "user-journeys" || key === "user-journeys-by-username";
        },
      });
      queryClient.invalidateQueries({ queryKey: ["default-active-journey"] });
      queryClient.invalidateQueries({ queryKey: ["journey-limits"] });
      closeDeleteJourneyConfirm();
    },
  });

  const handleDelete = () => {
    if (journeyId) {
      deleteMutation.mutate(journeyId);
    }
  };

  return (
    <Modal
      opened={isOpen}
      onClose={closeDeleteJourneyConfirm}
      title="Ta bort resa"
      centered
    >
      <Stack gap="lg">
        <Text>
          Är du säker på att du vill ta bort{" "}
          <strong>{journey?.title ?? "resan"}</strong>?
        </Text>

        <Text size="sm" c="orange">
          Resan och alla dess inlägg kommer att tas bort permanent.
        </Text>

        {journey && Number(journey.postCount) > 0 && (
          <Text size="sm" c="orange">
            Den här resan har {journey.postCount}{" "}
            {Number(journey.postCount) === 1 ? "inlägg" : "inlägg"} som också
            tas bort.
          </Text>
        )}

        <Group justify="flex-end">
          <Button
            variant="subtle"
            onClick={closeDeleteJourneyConfirm}
            disabled={deleteMutation.isPending}
          >
            Avbryt
          </Button>
          <Button
            color="red"
            onClick={handleDelete}
            loading={deleteMutation.isPending}
          >
            Ta bort
          </Button>
        </Group>
      </Stack>
    </Modal>
  );
}
