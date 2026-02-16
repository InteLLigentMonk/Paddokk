import { Modal } from '@mantine/core'
import { useStore } from '@tanstack/react-store'
import { carsPageStore, closeAddCarModal } from '@/lib/stores/cars-page-store'
import { CarForm } from './car-form'

export function AddCarModal() {
  const isOpen = useStore(carsPageStore, (state) => state.modals.addCar)

  return (
    <Modal
      opened={isOpen}
      onClose={closeAddCarModal}
      title="Add New Car"
      size="lg"
      centered
    >
      <CarForm onSuccess={closeAddCarModal} onCancel={closeAddCarModal} />
    </Modal>
  )
}
