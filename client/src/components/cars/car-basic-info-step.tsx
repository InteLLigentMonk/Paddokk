import { useEffect, useMemo, useState } from "react"
import { Button, Group, NumberInput, Select, Stack, TextInput } from "@mantine/core"
import { useForm } from "@tanstack/react-form"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { carsGetCarMakes, carsGetCarModels, carsGetCarGenerations } from "@/generated/api/cars/cars"
import { userCarsCreateUserCar, userCarsUpdateUserCar } from "@/generated/api/user-cars/user-cars"
import type { CreateUserCarCommand, UserCarDto } from "@/generated/api/schemas"
import { useNotifications } from "@/integrations/mantine"



interface CarBasicInfoStepProps {
  carId: number | null // If editing after going back from Step 2
  onNext: (carId: number) => void // Called after successful car creation
  onCancel: () => void
}

export function CarBasicInfoStep({ carId, onNext, onCancel }: CarBasicInfoStepProps) {
  const notifications = useNotifications()
  const queryClient = useQueryClient()

  const [selectedMakeId, setSelectedMakeId] = useState<number | undefined>()
  const [selectedModelId, setSelectedModelId] = useState<number | undefined>()

  const { data: makesData } = useQuery({
    queryKey: ["car-makes"],
    queryFn: () => carsGetCarMakes(),
  })
  const { data: modelsData } = useQuery({
    queryKey: ["car-models", selectedMakeId],
    queryFn: () => carsGetCarModels(selectedMakeId!),
    enabled: !!selectedMakeId,
  })
  const { data: generationsData } = useQuery({
    queryKey: ["car-generations", selectedModelId],
    queryFn: () => carsGetCarGenerations(selectedModelId!),
    enabled: !!selectedModelId,
  })

  const makes = makesData?.status === 200 ? makesData.data.makes : []
  const models = modelsData?.status === 200 ? modelsData.data.models : []
  const generations = generationsData?.status === 200 ? generationsData.data.generations : []

  const makesSelectData = useMemo(
    () => makes.map((make) => ({ value: make.id.toString(), label: make.name })),
    [makes],
  )
  const modelsSelectData = useMemo(
    () => models.map((model) => ({ value: model.id.toString(), label: model.name })),
    [models],
  )
  const generationsSelectData = useMemo(
    () => generations.map((gen) => ({ value: gen.id.toString(), label: gen.name })),
    [generations],
  )

  const addMutation = useMutation({
    mutationFn: (payload: Omit<CreateUserCarCommand, "subscriptionTier">) =>
      userCarsCreateUserCar(payload as CreateUserCarCommand),
  })

  const editMutation = useMutation({
    mutationFn: ({ id, nickname, color }: { id: number; nickname: string | null; color: string | null }) =>
      userCarsUpdateUserCar(id, { carId: id, nickname, color, description: null, isPrimary: null }),
  })

  const form = useForm({
    defaultValues: {
      carMakeId: 0,
      carModelId: 0,
      carGenerationId: undefined as number | null | undefined,
      year: new Date().getFullYear(),
      nickname: "",
      color: "",
    },
    onSubmit: async ({ value }) => {
      try {
        if (carId) {
          await editMutation.mutateAsync({
            id: carId,
            nickname: value.nickname || null,
            color: value.color || null,
          })
          notifications.success({ message: "Car updated!" })
          queryClient.invalidateQueries({ queryKey: ["user-cars"] })
          onNext(carId)
        } else {
          const result = await addMutation.mutateAsync({
            carMakeId: value.carMakeId,
            carModelId: value.carModelId,
            carGenerationId: value.carGenerationId || null,
            year: value.year,
            nickname: value.nickname || null,
            color: value.color || null,
            description: null,
            isPrimary: false,
          })
          queryClient.invalidateQueries({ queryKey: ["user-cars"] })
          notifications.success({ message: "Car created! Now add photos." })
          const car = result.data as UserCarDto
          onNext(Number(car.id))
        }
      } catch {
        notifications.error({
          message: carId ? "Failed to update car" : "Failed to create car",
        })
      }
    },
  })

  useEffect(() => {
    if (selectedMakeId) {
      form.setFieldValue("carModelId", 0)
      form.setFieldValue("carGenerationId", undefined)
      setSelectedModelId(undefined)
    }
  }, [selectedMakeId])

  useEffect(() => {
    if (selectedModelId) {
      form.setFieldValue("carGenerationId", undefined)
    }
  }, [selectedModelId])

  const isPending = addMutation.isPending || editMutation.isPending

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        form.handleSubmit()
      }}
    >
      <Stack gap="md" mt="md">
        <form.Field
          name="carMakeId"
          validators={{
            onChange: ({ value }) => (value < 1 ? "Please select a make" : undefined),
          }}
        >
          {(field) => (
            <Select
              label="Make"
              placeholder="Select make"
              data={makesSelectData}
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const numValue = value ? parseInt(value) : 0
                field.handleChange(numValue)
                setSelectedMakeId(numValue || undefined)
              }}
              error={field.state.meta.errors.join(", ")}
              searchable
              required
            />
          )}
        </form.Field>

        <form.Field
          name="carModelId"
          validators={{
            onChange: ({ value }) => (value < 1 ? "Please select a model" : undefined),
          }}
        >
          {(field) => (
            <Select
              label="Model"
              placeholder="Select model"
              data={modelsSelectData}
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const numValue = value ? parseInt(value) : 0
                field.handleChange(numValue)
                setSelectedModelId(numValue || undefined)
              }}
              error={field.state.meta.errors.join(", ")}
              disabled={!selectedMakeId}
              searchable
              required
            />
          )}
        </form.Field>

        <form.Field name="carGenerationId">
          {(field) => (
            <Select
              label="Generation (optional)"
              placeholder="Select generation"
              data={generationsSelectData}
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                field.handleChange(value ? parseInt(value) : undefined)
              }}
              disabled={!selectedModelId}
              searchable
              clearable
            />
          )}
        </form.Field>

        <form.Field
          name="year"
          validators={{
            onChange: ({ value }) => {
              if (value < 1900) return "Year must be 1900 or later"
              if (value > 2030) return "Year must be 2030 or earlier"
              return undefined
            },
          }}
        >
          {(field) => (
            <NumberInput
              label="Year"
              placeholder="Enter year"
              value={field.state.value}
              onChange={(value) => field.handleChange(Number(value))}
              error={field.state.meta.errors.join(", ")}
              min={1900}
              max={2030}
              required
            />
          )}
        </form.Field>

        <form.Field name="nickname">
          {(field) => (
            <TextInput
              label="Nickname (optional)"
              placeholder="Enter nickname"
              value={field.state.value ?? ""}
              onChange={(e) => field.handleChange(e.target.value)}
            />
          )}
        </form.Field>

        <form.Field name="color">
          {(field) => (
            <TextInput
              label="Color (optional)"
              placeholder="Enter color"
              value={field.state.value ?? ""}
              onChange={(e) => field.handleChange(e.target.value)}
            />
          )}
        </form.Field>

        <Group justify="flex-end" mt="md">
          <Button variant="subtle" onClick={onCancel} disabled={isPending}>
            Cancel
          </Button>
          <Button
            type="submit"
            loading={isPending}
            disabled={form.state.values.carMakeId < 1 || form.state.values.carModelId < 1}
          >
            {carId ? "Update & Continue" : "Next"}
          </Button>
        </Group>
      </Stack>
    </form>
  )
}
