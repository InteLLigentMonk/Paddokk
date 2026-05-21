import { useMemo, useState } from "react"
import { Button, Group, NumberInput, Select, Stack, Switch, TextInput } from "@mantine/core"
import { useForm } from "@tanstack/react-form"
import { useQuery } from "@tanstack/react-query"
import type { CarBasicFormData } from "./car-form-stepper"
import { carsGetCarGenerations, carsGetCarMakes, carsGetCarModels } from "@/generated/api/cars/cars"

interface CarBasicInfoStepProps {
  initialData: CarBasicFormData | null
  onNext: (data: CarBasicFormData) => void
  onCancel: () => void
}

export function CarBasicInfoStep({ initialData, onNext, onCancel }: CarBasicInfoStepProps) {
  const [isCustomBuild, setIsCustomBuild] = useState(initialData?.isCustomBuild ?? false)
  const [customBuildName, setCustomBuildName] = useState(initialData?.customBuildName ?? "")
  const [selectedMakeId, setSelectedMakeId] = useState<number | undefined>(
    initialData?.carMakeId ?? undefined,
  )
  const [selectedModelId, setSelectedModelId] = useState<number | undefined>(
    initialData?.carModelId ?? undefined,
  )
  const [selectedGenerationId, setSelectedGenerationId] = useState<number | undefined>(
    initialData?.carGenerationId ?? undefined,
  )

  const { data: makesData } = useQuery({
    queryKey: ["car-makes"],
    queryFn: () => carsGetCarMakes(),
    enabled: !isCustomBuild,
  })
  const { data: modelsData } = useQuery({
    queryKey: ["car-models", selectedMakeId],
    queryFn: () => carsGetCarModels(selectedMakeId!),
    enabled: !isCustomBuild && !!selectedMakeId,
  })
  const { data: generationsData } = useQuery({
    queryKey: ["car-generations", selectedModelId],
    queryFn: () => carsGetCarGenerations(selectedModelId!),
    enabled: !isCustomBuild && !!selectedModelId,
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
      customBuildName: initialData?.customBuildName ?? "",
      carMakeId: initialData?.carMakeId ?? 0,
      carModelId: initialData?.carModelId ?? 0,
      carGenerationId: initialData?.carGenerationId ?? (undefined as number | null | undefined),
      year: initialData?.year ?? new Date().getFullYear(),
      nickname: initialData?.nickname ?? "",
      color: initialData?.color ?? "",
    },
    onSubmit: ({ value }) => {
      if (isCustomBuild) {
        onNext({
          isCustomBuild: true,
          customBuildName: value.customBuildName || null,
          carMakeId: null,
          carModelId: null,
          carGenerationId: null,
          year: null,
          nickname: value.nickname || null,
          color: value.color || null,
        })
      } else {
        onNext({
          isCustomBuild: false,
          customBuildName: null,
          carMakeId: value.carMakeId,
          carModelId: value.carModelId,
          carGenerationId: value.carGenerationId ?? null,
          year: value.year,
          nickname: value.nickname || null,
          color: value.color || null,
        })
      }
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

  const isStandardValid = (selectedMakeId ?? 0) > 0 && (selectedModelId ?? 0) > 0
  const isCustomValid = customBuildName.trim().length > 0

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        form.handleSubmit()
      }}
    >
      <Stack gap="md" mt="md">
        <Switch
          label="Custom build"
          description="Enable if your car doesn't fit a standard make/model (e.g. swaps, one-offs)"
          checked={isCustomBuild}
          onChange={(e) => setIsCustomBuild(e.currentTarget.checked)}
        />

        {isCustomBuild ? (
          <form.Field
            name="customBuildName"
            validators={{
              onChange: ({ value }) =>
                !value.trim() ? "Custom build name is required" : undefined,
            }}
          >
            {(field) => (
              <TextInput
                label="Build name"
                placeholder="e.g. SR20DET S13, AE86 with 4AGE, custom turbo build"
                value={field.state.value}
                onChange={(e) => {
                  field.handleChange(e.target.value)
                  setCustomBuildName(e.target.value)
                }}
                error={field.state.meta.errors.join(", ")}
                required
              />
            )}
          </form.Field>
        ) : (
          <>
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
          </>
        )}

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

        <Group justify="flex-end" mt="md">
          <Button variant="subtle" onClick={onCancel}>
            Cancel
          </Button>
          <Button
            type="submit"
            disabled={isCustomBuild ? !isCustomValid : !isStandardValid}
          >
            Next
          </Button>
        </Group>
      </Stack>
    </form>
  )
}
