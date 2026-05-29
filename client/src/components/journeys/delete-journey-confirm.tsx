import { Button, Group, Modal, Stack, Text } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  closeDeleteJourneyConfirm,
  journeysPageStore,
} from "@/lib/stores/journeys-page-store";
import { userJourneysGetUserJourney } from "@/generated/api/user-journeys/user-journeys";
import {
  deleteUserJourneyFn,
  getDefaultActiveJourneyFn,
} from "@/lib/api/user-journeys";
import { journeyKeys } from "@/lib/api/journeys.keys";
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
    queryKey: journeyKeys.userJourney(journeyId),
    queryFn: () => userJourneysGetUserJourney(journeyId!),
    enabled: isOpen && !!journeyId,
  });

  const { data: defaultJourney } = useQuery({
    queryKey: journeyKeys.defaultActiveJourney,
    queryFn: () => getDefaultActiveJourneyFn(),
    enabled: isOpen,
  });

  const journey = data;
  const isDefault = defaultJourney
    ? Number(defaultJourney.id) === journeyId
    : false;

  const deleteMutation = useMutation({
    mutationFn: (id: number) =>
      deleteUserJourneyFn({ data: { journeyId: id } }),
    onError: () => {
      notifications.error({
        message: "Kunde inte ta bort resan. FÃ¶rsÃ¶k igen.",
      });
    },
    onSuccess: () => {
      notifications.success({
        message: isDefault
          ? "Resan borttagen, aktiv resa uppdaterad"
          : "Resan borttagen",
      });
      journeyKeys.userJourneyListRoots.forEach((queryKey) =>
        queryClient.invalidateQueries({ queryKey }),
      );
      queryClient.invalidateQueries({
        queryKey: journeyKeys.defaultActiveJourney,
      });
      queryClient.invalidateQueries({ queryKey: journeyKeys.journeyLimits });
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
          Ã„r du sÃ¤ker pÃ¥ att du vill ta bort{" "}
          <strong>{journey?.title ?? "resan"}</strong>?
        </Text>

        <Text size="sm" c="orange">
          Resan och alla dess inlÃ¤gg kommer att tas bort permanent.
        </Text>

        {journey && Number(journey.postCount) > 0 && (
          <Text size="sm" c="orange">
            Den hÃ¤r resan har {journey.postCount}{" "}
            {Number(journey.postCount) === 1 ? "inlÃ¤gg" : "inlÃ¤gg"} som ocksÃ¥
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
