import { useState } from "react"
import { Stepper } from "@mantine/core"
import { CarBasicInfoStep } from "./car-basic-info-step"
import { CarImagesStep } from "./car-images-step"

interface CarFormStepperProps {
  onSuccess: () => void // Closes modal
  onCancel: () => void // Closes modal
}

export function CarFormStepper({ onSuccess, onCancel }: CarFormStepperProps) {
  const [activeStep, setActiveStep] = useState(0)
  const [createdCarId, setCreatedCarId] = useState<number | null>(null)

  // TODO: Handle step navigation
  const handleNextStep = (carId: number) => {
    setCreatedCarId(carId)
    setActiveStep(1)
  }

  const handleBackToStep1 = () => {
    setActiveStep(0)
  }

  const handleFinish = () => {
    onSuccess()
  }

  return (
    <Stepper
      active={activeStep}
      onStepClick={(step) => {
        // Only allow clicking on completed steps
        if (step === 0 && createdCarId) {
          setActiveStep(step)
        }
      }}
      allowNextStepsSelect={false}
    >
      <Stepper.Step
        label="Car Details"
        description="Basic information"
        allowStepSelect={activeStep > 0}
      >
        <CarBasicInfoStep
          carId={createdCarId}
          onNext={handleNextStep}
          onCancel={onCancel}
        />
      </Stepper.Step>

      <Stepper.Step
        label="Photos"
        description="Upload images"
        allowStepSelect={false}
      >
        {createdCarId && (
          <CarImagesStep
            carId={createdCarId}
            onFinish={handleFinish}
            onBack={handleBackToStep1}
          />
        )}
      </Stepper.Step>
    </Stepper>
  )
}
