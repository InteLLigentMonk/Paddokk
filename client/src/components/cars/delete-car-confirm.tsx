import { Button, Group, Modal, Stack, Text } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  carsPageStore,
  closeDeleteCarConfirm,
} from "@/lib/stores/cars-page-store";
import {
  userCarsDeleteUserCar,
  userCarsGetUserCar,
} from "@/generated/api/user-cars/user-cars";
import { carKeys } from "@/lib/api/cars.keys";
import { useNotifications } from "@/integrations/mantine";

export function DeleteCarConfirm() {
  const deleteCarState = useStore(
    carsPageStore,
    (state) => state.modals.deleteCar,
  );
  const { open: isOpen, carId } = deleteCarState;
  const queryClient = useQueryClient();
  const notifications = useNotifications();

  const { data } = useQuery({
    queryKey: carKeys.userCar(carId),
    queryFn: () => userCarsGetUserCar(carId!),
    enabled: isOpen && !!carId,
  });

  const car = data;

  const deleteMutation = useMutation({
    meta: { suppressGlobalError: true },
    mutationFn: (id: number) => userCarsDeleteUserCar(id),
    onError: () => {
      notifications.error({
        message: "Failed to delete car. Please try again.",
      });
    },
    onSuccess: () => {
      notifications.success({ message: "Car deleted successfully" });
      carKeys.userCarListRoots.forEach((queryKey) =>
        queryClient.invalidateQueries({ queryKey }),
      );
      queryClient.invalidateQueries({ queryKey: carKeys.carLimits });
      closeDeleteCarConfirm();
    },
  });

  const handleDelete = () => {
    if (carId) {
      deleteMutation.mutate(carId);
    }
  };

  const displayName =
    car?.nickname || `${car?.carMakeName} ${car?.carModelName}`;

  return (
    <Modal
      opened={isOpen}
      onClose={closeDeleteCarConfirm}
      title="Delete Car"
      centered
    >
      <Stack gap="lg">
        <Text>
          Are you sure you want to delete <strong>{displayName}</strong>? This
          action cannot be undone.
        </Text>

        {car && Number(car.journeyCount) > 0 && (
          <Text size="sm" c="orange">
            Warning: This car has {car.journeyCount}{" "}
            {car.journeyCount === 1 ? "journey" : "journeys"} associated with
            it.
          </Text>
        )}

        <Group justify="flex-end">
          <Button
            variant="subtle"
            onClick={closeDeleteCarConfirm}
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
