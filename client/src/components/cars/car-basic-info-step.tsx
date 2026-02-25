import { useEffect, useMemo, useState } from "react"
import { Button, Group, NumberInput, Select, Stack, TextInput } from "@mantine/core"
import { useForm } from "@tanstack/react-form"
import { z } from "zod"
import {
  useGetApiCarsMakes,
  useGetApiCarsMakesMakeIdModels,
  useGetApiCarsModelsModelIdGenerations,
} from "@/generated/api/cars/cars"
import {
  usePostApiUsersMeCars,
  usePutApiUsersMeCarsCarId,
  getGetApiUsersMeCarsQueryKey,
} from "@/generated/api/user-cars/user-cars"
import { useNotifications } from "@/integrations/mantine"
import { useQueryClient } from "@tanstack/react-query"

// Step 1 schema matching API requirements
const stepOneSchema = z.object({
  carMakeId: z.number().min(1, "Please select a make"),
  carModelId: z.number().min(1, "Please select a model"),
  carGenerationId: z.number().optional().nullable(),
  year: z.number().min(1900, "Year must be 1900 or later").max(2030, "Year must be 2030 or earlier"),
  nickname: z.string().max(100).optional().nullable(),
  color: z.string().max(50).optional().nullable(),
})

type StepOneFormValues = z.infer<typeof stepOneSchema>

interface CarBasicInfoStepProps {
  carId: number | null // If editing after going back from Step 2
  onNext: (carId: number) => void // Called after successful car creation
  onCancel: () => void
}

export function CarBasicInfoStep({ carId, onNext, onCancel }: CarBasicInfoStepProps) {
  const notifications = useNotifications()
  const queryClient = useQueryClient()

  // TODO: Extract cascading dropdowns logic from car-form.tsx
  const [selectedMakeId, setSelectedMakeId] = useState<number | undefined>()
  const [selectedModelId, setSelectedModelId] = useState<number | undefined>()

  // Fetch makes, models, generations
  const { data: makesData } = useGetApiCarsMakes()
  const { data: modelsData } = useGetApiCarsMakesMakeIdModels(
    selectedMakeId ?? 0,
    {
      query: { enabled: !!selectedMakeId },
    }
  )
  const { data: generationsData } = useGetApiCarsModelsModelIdGenerations(
    selectedModelId ?? 0,
    {
      query: { enabled: !!selectedModelId },
    }
  )

  // Handle both array (direct DTO) and object ({data, status}) response formats
  const makes = Array.isArray(makesData) ? makesData : (makesData?.data ?? [])
  const models = Array.isArray(modelsData) ? modelsData : (modelsData?.data ?? [])
  const generations = Array.isArray(generationsData) ? generationsData : (generationsData?.data ?? [])

  // Transform data for Select components
  const makesSelectData = useMemo(() => {
    return makes.map((make) => ({
      value: make.id!.toString(),
      label: make.name!,
    }))
  }, [makes])

  const modelsSelectData = useMemo(() => {
    return models.map((model) => ({
      value: model.id!.toString(),
      label: model.name!,
    }))
  }, [models])

  const generationsSelectData = useMemo(() => {
    return generations.map((gen) => ({
      value: gen.id!.toString(),
      label: gen.name!,
    }))
  }, [generations])

  // TODO: Setup mutations (POST for create, PUT for edit)
  const addMutation = usePostApiUsersMeCars()
  const editMutation = usePutApiUsersMeCarsCarId()

  // TODO: Setup TanStack Form
  const form = useForm<StepOneFormValues>({
    defaultValues: {
      carMakeId: 0,
      carModelId: 0,
      carGenerationId: undefined,
      year: new Date().getFullYear(),
      nickname: "",
      color: "",
    },
    onSubmit: async ({ value }) => {
      try {
        // Prepare JSON payload matching CreateUserCarRequest
        const payload = {
          carMakeId: value.carMakeId,
          carModelId: value.carModelId,
          carGenerationId: value.carGenerationId || null,
          year: value.year,
          nickname: value.nickname || null,
          color: value.color || null,
          description: null, // Not in Step 1
          isPrimary: false, // Not relevant for multi-step flow
        }

        let carResponse
        if (carId) {
          // Update existing car
          carResponse = await editMutation.mutateAsync({
            carId,
            data: payload,
          })
          notifications.success({ message: "Car updated!" })
        } else {
          // Create new car
          carResponse = await addMutation.mutateAsync({
            data: payload,
          })
          notifications.success({ message: "Car created! Now add photos." })
        }

        queryClient.invalidateQueries({ queryKey: getGetApiUsersMeCarsQueryKey() })
        onNext(carResponse.id)
      } catch (error) {
        notifications.error({
          message: carId ? "Failed to update car" : "Failed to create car",
        })
      }
    },
  })

  // TODO: Reset dependent fields when parent changes
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

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        form.handleSubmit()
      }}
    >
      <Stack gap="md" mt="md">
        {/* TODO: Render Make dropdown */}
        <form.Field
          name="carMakeId"
          validators={{
            onChange: ({ value }) =>
              value < 1 ? "Please select a make" : undefined,
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

        {/* TODO: Render Model dropdown */}
        <form.Field
          name="carModelId"
          validators={{
            onChange: ({ value }) =>
              value < 1 ? "Please select a model" : undefined,
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

        {/* TODO: Render Generation dropdown */}
        <form.Field name="carGenerationId">
          {(field) => (
            <Select
              label="Generation (optional)"
              placeholder="Select generation"
              data={generationsSelectData}
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const numValue = value ? parseInt(value) : undefined
                field.handleChange(numValue)
              }}
              disabled={!selectedModelId}
              searchable
              clearable
            />
          )}
        </form.Field>

        {/* TODO: Render Year, Nickname, Color fields */}
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
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
            />
          )}
        </form.Field>

        <form.Field name="color">
          {(field) => (
            <TextInput
              label="Color (optional)"
              placeholder="Enter color"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
            />
          )}
        </form.Field>

        {/* Cancel and Next buttons */}
        <Group justify="flex-end" mt="md">
          <Button
            variant="subtle"
            onClick={onCancel}
            disabled={addMutation.isPending || editMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            type="submit"
            loading={addMutation.isPending || editMutation.isPending}
            disabled={
              form.state.values.carMakeId < 1 || form.state.values.carModelId < 1
            }
          >
            {carId ? "Update & Continue" : "Next"}
          </Button>
        </Group>
      </Stack>
    </form>
  )
}
