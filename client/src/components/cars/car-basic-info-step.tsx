import { useMemo, useState } from "react"
import { Button, Group, NumberInput, Select, Stack, TextInput } from "@mantine/core"
import { useForm } from "@tanstack/react-form"
import { useQuery } from "@tanstack/react-query"
import { carsGetCarMakes, carsGetCarModels, carsGetCarGenerations } from "@/generated/api/cars/cars"
import type { CarBasicFormData } from "./car-form-stepper"

interface CarBasicInfoStepProps {
  initialData: CarBasicFormData | null
  onNext: (data: CarBasicFormData) => void
  onCancel: () => void
}

export function CarBasicInfoStep({ initialData, onNext, onCancel }: CarBasicInfoStepProps) {
  const [selectedMakeId, setSelectedMakeId] = useState<number | undefined>(
    initialData?.carMakeId || undefined,
  )
  const [selectedModelId, setSelectedModelId] = useState<number | undefined>(
    initialData?.carModelId || undefined,
  )
  const [selectedGenerationId, setSelectedGenerationId] = useState<number | undefined>(
    initialData?.carGenerationId ?? undefined,
  )

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

  const form = useForm({
    defaultValues: {
      carMakeId: initialData?.carMakeId ?? 0,
      carModelId: initialData?.carModelId ?? 0,
      carGenerationId: initialData?.carGenerationId ?? (undefined as number | null | undefined),
      year: initialData?.year ?? new Date().getFullYear(),
      nickname: initialData?.nickname ?? "",
      color: initialData?.color ?? "",
    },
    onSubmit: ({ value }) => {
      onNext({
        carMakeId: value.carMakeId,
        carModelId: value.carModelId,
        carGenerationId: value.carGenerationId ?? null,
        year: value.year,
        nickname: value.nickname || null,
        color: value.color || null,
      })
    },
  })

  const yearSelectData = useMemo(() => {
    const gen = selectedGenerationId
      ? generations.find((g) => g.id.toString() === selectedGenerationId.toString())
      : undefined
    if (!gen) return []
    const start = Number(gen.startYear)
    const end = Number(gen.endYear ?? new Date().getFullYear())
    return Array.from({ length: end - start + 1 }, (_, i) => end - i).map((y) => ({
      value: y.toString(),
      label: y.toString(),
    }))
  }, [generations, selectedGenerationId])

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
                form.setFieldValue("carModelId", 0)
                form.setFieldValue("carGenerationId", undefined)
                setSelectedModelId(undefined)
                setSelectedGenerationId(undefined)
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
                form.setFieldValue("carGenerationId", undefined)
                setSelectedGenerationId(undefined)
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
                const numValue = value ? parseInt(value) : undefined
                field.handleChange(numValue)
                setSelectedGenerationId(numValue)
                const gen = value ? generations.find((g) => g.id.toString() === value) : undefined
                if (gen) {
                  const defaultYear = Number(gen.endYear ?? new Date().getFullYear())
                  form.setFieldValue("year", defaultYear)
                }
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
              if (value > new Date().getFullYear() + 1) return "Invalid year"
              return undefined
            },
          }}
        >
          {(field) =>
            yearSelectData.length > 0 ? (
              <Select
                label="Year"
                placeholder="Select year"
                data={yearSelectData}
                value={field.state.value ? field.state.value.toString() : null}
                onChange={(value) => field.handleChange(Number(value))}
                error={field.state.meta.errors.join(", ")}
                required
              />
            ) : (
              <NumberInput
                label="Year"
                placeholder="Enter year"
                value={field.state.value}
                onChange={(value) => field.handleChange(Number(value))}
                error={field.state.meta.errors.join(", ")}
                min={1900}
                max={new Date().getFullYear() + 1}
                required
              />
            )
          }
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
          <Button variant="subtle" onClick={onCancel}>
            Cancel
          </Button>
          <Button
            type="submit"
            disabled={form.state.values.carMakeId < 1 || form.state.values.carModelId < 1}
          >
            Next
          </Button>
        </Group>
      </Stack>
    </form>
  )
}
