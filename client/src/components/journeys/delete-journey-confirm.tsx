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
    meta: { suppressGlobalError: true },
    mutationFn: (id: number) =>
      deleteUserJourneyFn({ data: { journeyId: id } }),
    onError: () => {
      notifications.error({
        message: "Could not delete the journey. Please try again.",
      });
    },
    onSuccess: () => {
      notifications.success({
        message: isDefault
          ? "Journey deleted, active journey updated"
          : "Journey deleted",
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
      title="Delete journey"
      centered
    >
      <Stack gap="lg">
        <Text>
          Are you sure you want to delete{" "}
          <strong>{journey?.title ?? "this journey"}</strong>?
        </Text>

        <Text size="sm" c="orange">
          The journey and all its posts will be permanently deleted.
        </Text>

        {journey && Number(journey.postCount) > 0 && (
          <Text size="sm" c="orange">
            This journey has {journey.postCount}{" "}
            {Number(journey.postCount) === 1 ? "post" : "posts"} that will also
            be deleted.
          </Text>
        )}

        <Group justify="flex-end">
          <Button
            variant="subtle"
            onClick={closeDeleteJourneyConfirm}
            disabled={deleteMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            color="red"
            onClick={handleDelete}
            loading={deleteMutation.isPending}
          >
            Delete
          </Button>
        </Group>
      </Stack>
    </Modal>
  );
}
