import { Modal, Loader, Center } from '@mantine/core'
import { useStore } from '@tanstack/react-store'
import { carsPageStore, closeEditCarModal } from '@/lib/stores/cars-page-store'
import { useGetApiUsersMeCarsCarId } from '@/generated/api/user-cars/user-cars'
import { CarForm } from './car-form'

export function EditCarModal() {
  const editCarState = useStore(carsPageStore, (state) => state.modals.editCar)
  const { open: isOpen, carId } = editCarState

  const { data, isLoading } = useGetApiUsersMeCarsCarId(carId!, {
    query: { enabled: isOpen && !!carId },
  })

  return (
    <Modal
      opened={isOpen}
      onClose={closeEditCarModal}
      title="Edit Car"
      size="lg"
      centered
    >
      {isLoading ? (
        <Center py="xl">
          <Loader />
        </Center>
      ) : data?.data ? (
        <CarForm
          initialValues={data.data}
          carId={carId!}
          onSuccess={closeEditCarModal}
          onCancel={closeEditCarModal}
        />
      ) : (
        <Center py="xl">Car not found</Center>
      )}
    </Modal>
  )
}
