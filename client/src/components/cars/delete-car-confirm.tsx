import { Modal, Text, Button, Group, Stack } from '@mantine/core'
import { useStore } from '@tanstack/react-store'
import { carsPageStore, closeDeleteCarConfirm } from '@/lib/stores/cars-page-store'
import {
  useDeleteApiUsersMeCarsCarId,
  getGetApiUsersMeCarsQueryKey,
  useGetApiUsersMeCarsCarId,
} from '@/generated/api/user-cars/user-cars'
import { useQueryClient } from '@tanstack/react-query'
import { useNotifications } from '@/integrations/mantine'

export function DeleteCarConfirm() {
  const deleteCarState = useStore(carsPageStore, (state) => state.modals.deleteCar)
  const { open: isOpen, carId } = deleteCarState
  const queryClient = useQueryClient()
  const notifications = useNotifications()

  const { data } = useGetApiUsersMeCarsCarId(carId!, {
    query: { enabled: isOpen && !!carId },
  })

  const car = data?.data

  const deleteMutation = useDeleteApiUsersMeCarsCarId({
    mutation: {
      onMutate: async ({ carId }) => {
        const queryKey = getGetApiUsersMeCarsQueryKey()
        await queryClient.cancelQueries({ queryKey })
        const previous = queryClient.getQueryData(queryKey)

        queryClient.setQueryData(queryKey, (old: any) => {
          if (!old?.data) return old
          return {
            ...old,
            data: old.data.filter((c: any) => c.id !== carId),
          }
        })

        return { previous, queryKey }
      },
      onError: (err, vars, context) => {
        if (context?.queryKey && context?.previous) {
          queryClient.setQueryData(context.queryKey, context.previous)
        }
        notifications.error({ message: 'Failed to delete car. Please try again.' })
      },
      onSuccess: () => {
        notifications.success({ message: 'Car deleted successfully' })
        closeDeleteCarConfirm()
      },
      onSettled: () => {
        queryClient.invalidateQueries({ queryKey: getGetApiUsersMeCarsQueryKey() })
      },
    },
  })

  const handleDelete = () => {
    if (carId) {
      deleteMutation.mutate({ carId })
    }
  }

  const displayName = car?.nickname || `${car?.carMakeName} ${car?.carModelName}`

  return (
    <Modal
      opened={isOpen}
      onClose={closeDeleteCarConfirm}
      title="Delete Car"
      centered
    >
      <Stack gap="lg">
        <Text>
          Are you sure you want to delete <strong>{displayName}</strong>? This action cannot be
          undone.
        </Text>

        {car?.journeyCount !== undefined && car.journeyCount > 0 && (
          <Text size="sm" c="orange">
            Warning: This car has {car.journeyCount}{' '}
            {car.journeyCount === 1 ? 'journey' : 'journeys'} associated with it.
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
          <Button color="red" onClick={handleDelete} loading={deleteMutation.isPending}>
            Delete
          </Button>
        </Group>
      </Stack>
    </Modal>
  )
}
