import { useState, useEffect, useRef } from "react"
import { Box, Stepper } from "@mantine/core"
import { Car, Camera, Check } from "lucide-react"
import { useQueryClient } from "@tanstack/react-query"
import { CarBasicInfoStep } from "./car-basic-info-step"
import { CarImagesStep } from "./car-images-step"
import { userCarsCreateUserCar } from "@/generated/api/user-cars/user-cars"
import { carImagesUploadCarImage } from "@/generated/api/car-images/car-images"
import { useNotifications } from "@/integrations/mantine"
import type { UserCarDto, CreateUserCarCommand } from "@/generated/api/schemas"

export interface PendingImage {
  localId: string
  file: File
  previewUrl: string
  isPrimary: boolean
}

export interface CarBasicFormData {
  carMakeId: number
  carModelId: number
  carGenerationId: number | null
  year: number
  nickname: string | null
  color: string | null
}

interface CarFormStepperProps {
  onSuccess: () => void
  onCancel: () => void
}

export function CarFormStepper({ onSuccess, onCancel }: CarFormStepperProps) {
  const [activeStep, setActiveStep] = useState(0)
  const [carFormData, setCarFormData] = useState<CarBasicFormData | null>(null)
  const [pendingImages, setPendingImages] = useState<PendingImage[]>([])
  const [isSubmitting, setIsSubmitting] = useState(false)
  const pendingImagesRef = useRef<PendingImage[]>([])
  const queryClient = useQueryClient()
  const notifications = useNotifications()

  useEffect(() => {
    pendingImagesRef.current = pendingImages
  }, [pendingImages])

  useEffect(() => {
    return () => {
      pendingImagesRef.current.forEach((img) => URL.revokeObjectURL(img.previewUrl))
    }
  }, [])

  const handleNextStep = (data: CarBasicFormData) => {
    setCarFormData(data)
    setActiveStep(1)
  }

  const handleBackToStep1 = () => {
    setActiveStep(0)
  }

  const handleFinish = async () => {
    if (!carFormData) return
    setIsSubmitting(true)
    try {
      const result = await userCarsCreateUserCar({
        carMakeId: carFormData.carMakeId,
        carModelId: carFormData.carModelId,
        carGenerationId: carFormData.carGenerationId,
        year: carFormData.year,
        nickname: carFormData.nickname,
        color: carFormData.color,
        description: null,
        isPrimary: false,
      } as CreateUserCarCommand)
      const car = result.data as UserCarDto
      const carId = Number(car.id)
      for (const img of pendingImages) {
        await carImagesUploadCarImage(carId, { File: img.file })
      }
      queryClient.invalidateQueries({ queryKey: ["user-cars"] })
      queryClient.invalidateQueries({ queryKey: ["car-limits"] })
      notifications.success({ message: "Car added successfully!" })
      onSuccess()
    } catch (err) {
      console.error("Failed to add car:", err)
      notifications.error({ message: "Failed to add car" })
      setIsSubmitting(false)
    }
  }

  return (
    <Stepper
      active={activeStep}
      onStepClick={(step) => {
        if (step === 0 && carFormData) {
          setActiveStep(step)
        }
      }}
      allowNextStepsSelect={false}
    >
      <Stepper.Step
        label={<Box visibleFrom="sm">Car Details</Box>}
        description={<Box visibleFrom="sm">Basic information</Box>}
        icon={<Car size={18} />}
        completedIcon={<Check size={18} />}
        allowStepSelect={activeStep > 0}
      >
        {activeStep === 0 && (
          <CarBasicInfoStep
            initialData={carFormData}
            onNext={handleNextStep}
            onCancel={onCancel}
          />
        )}
      </Stepper.Step>

      <Stepper.Step
        label={<Box visibleFrom="sm">Photos</Box>}
        description={<Box visibleFrom="sm">Upload images</Box>}
        icon={<Camera size={18} />}
        completedIcon={<Check size={18} />}
        allowStepSelect={false}
      >
        {activeStep === 1 && (
          <CarImagesStep
            pendingImages={pendingImages}
            onImagesChange={setPendingImages}
            isSubmitting={isSubmitting}
            onFinish={handleFinish}
            onBack={handleBackToStep1}
          />
        )}
      </Stepper.Step>
    </Stepper>
  )
}
