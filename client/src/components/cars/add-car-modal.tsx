import { Modal } from '@mantine/core'
import { useStore } from '@tanstack/react-store'
import { carsPageStore, closeAddCarModal } from '@/lib/stores/cars-page-store'
import { CarFormStepper } from './car-form-stepper'

export function AddCarModal() {
  const isOpen = useStore(carsPageStore, (state) => state.modals.addCar)

  return (
    <Modal
      opened={isOpen}
      onClose={closeAddCarModal}
      title="Add New Car"
      size="xl"
      centered
    >
      <CarFormStepper onSuccess={closeAddCarModal} onCancel={closeAddCarModal} />
    </Modal>
  )
}
