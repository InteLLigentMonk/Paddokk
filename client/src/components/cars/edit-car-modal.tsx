import { Center, Loader, Modal } from '@mantine/core'
import { useStore } from '@tanstack/react-store'
import { useQuery } from '@tanstack/react-query'
import { CarForm } from './car-form'
import { carsPageStore, closeEditCarModal } from '@/lib/stores/cars-page-store'
import { userCarsGetUserCar } from '@/generated/api/user-cars/user-cars'

export function EditCarModal() {
  const editCarState = useStore(carsPageStore, (state) => state.modals.editCar)
  const { open: isOpen, carId } = editCarState

  const { data, isLoading } = useQuery({
    queryKey: ['user-car', carId],
    queryFn: () => userCarsGetUserCar(carId!),
    enabled: isOpen && !!carId,
  })

  const car = data?.status === 200 ? data.data : undefined

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
      ) : car ? (
        <CarForm
          initialValues={car}
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
