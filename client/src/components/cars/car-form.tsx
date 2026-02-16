import { useForm } from '@tanstack/react-form'
import { z } from 'zod'
import { Stack, TextInput, NumberInput, Textarea, Button, Group, Select, Checkbox } from '@mantine/core'
import { useState, useEffect } from 'react'
import {
  useGetApiCarsMakes,
  useGetApiCarsMakesMakeIdModels,
  useGetApiCarsModelsModelIdGenerations,
} from '@/generated/api/cars/cars'
import {
  usePostApiUsersMeCars,
  usePutApiUsersMeCarsCarId,
  getGetApiUsersMeCarsQueryKey,
} from '@/generated/api/user-cars/user-cars'
import { useQueryClient } from '@tanstack/react-query'
import { useNotifications } from '@/integrations/mantine'
import type { UserCarDto } from '@/generated/api'
import { CarImageUpload } from './car-image-upload'

const carFormSchema = z.object({
  carMakeId: z.number().min(1, 'Please select a make'),
  carModelId: z.number().min(1, 'Please select a model'),
  carGenerationId: z.number().optional(),
  year: z.number().min(1900, 'Year must be 1900 or later').max(new Date().getFullYear() + 1, 'Invalid year'),
  nickname: z.string().optional(),
  color: z.string().optional(),
  description: z.string().optional(),
  isPrimary: z.boolean().optional(),
  primaryImage: z.instanceof(File).optional().or(z.string().optional()),
})

type CarFormValues = z.infer<typeof carFormSchema>

interface CarFormProps {
  initialValues?: UserCarDto
  carId?: number
  onSuccess: () => void
  onCancel: () => void
}

export function CarForm({ initialValues, carId, onSuccess, onCancel }: CarFormProps) {
  const isEditing = !!carId
  const queryClient = useQueryClient()
  const notifications = useNotifications()

  const [selectedMakeId, setSelectedMakeId] = useState<number | null>(
    initialValues?.carMakeId ?? null
  )
  const [selectedModelId, setSelectedModelId] = useState<number | null>(
    initialValues?.carModelId ?? null
  )

  const { data: makesData } = useGetApiCarsMakes()
  const { data: modelsData } = useGetApiCarsMakesMakeIdModels(selectedMakeId!, {
    query: { enabled: !!selectedMakeId },
  })
  const { data: generationsData } = useGetApiCarsModelsModelIdGenerations(selectedModelId!, {
    query: { enabled: !!selectedModelId },
  })

  const makes = makesData?.data ?? []
  const models = modelsData?.data ?? []
  const generations = generationsData?.data ?? []

  const addMutation = usePostApiUsersMeCars({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: getGetApiUsersMeCarsQueryKey() })
        notifications.success({ message: 'Car added successfully!' })
        onSuccess()
      },
    },
  })

  const editMutation = usePutApiUsersMeCarsCarId({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: getGetApiUsersMeCarsQueryKey() })
        notifications.success({ message: 'Car updated successfully!' })
        onSuccess()
      },
    },
  })

  const form = useForm({
    defaultValues: {
      carMakeId: initialValues?.carMakeId ?? 0,
      carModelId: initialValues?.carModelId ?? 0,
      carGenerationId: initialValues?.carGenerationId,
      year: initialValues?.year ?? new Date().getFullYear(),
      nickname: initialValues?.nickname ?? '',
      color: initialValues?.color ?? '',
      description: initialValues?.description ?? '',
      isPrimary: initialValues?.isPrimary ?? false,
      primaryImage: initialValues?.primaryImageUrl,
    } as CarFormValues,
    onSubmit: async ({ value }) => {
      const formData = new FormData()

      formData.append('carMakeId', value.carMakeId.toString())
      formData.append('carModelId', value.carModelId.toString())
      if (value.carGenerationId) {
        formData.append('carGenerationId', value.carGenerationId.toString())
      }
      formData.append('year', value.year.toString())
      if (value.nickname) {
        formData.append('nickname', value.nickname)
      }
      if (value.color) {
        formData.append('color', value.color)
      }
      if (value.description) {
        formData.append('description', value.description)
      }
      formData.append('isPrimary', value.isPrimary ? 'true' : 'false')

      if (value.primaryImage instanceof File) {
        formData.append('primaryImage', value.primaryImage)
      }

      if (isEditing) {
        await editMutation.mutateAsync({ carId: carId!, data: formData as any })
      } else {
        await addMutation.mutateAsync({ data: formData as any })
      }
    },
  })

  const isLoading = addMutation.isPending || editMutation.isPending

  // Reset model and generation when make changes
  useEffect(() => {
    if (selectedMakeId !== form.state.values.carMakeId && !isEditing) {
      form.setFieldValue('carModelId', 0)
      form.setFieldValue('carGenerationId', undefined)
      setSelectedModelId(null)
    }
  }, [selectedMakeId, form.state.values.carMakeId, isEditing])

  // Reset generation when model changes
  useEffect(() => {
    if (selectedModelId !== form.state.values.carModelId && !isEditing) {
      form.setFieldValue('carGenerationId', undefined)
    }
  }, [selectedModelId, form.state.values.carModelId, isEditing])

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        form.handleSubmit()
      }}
    >
      <Stack gap="md">
        <form.Field name="primaryImage">
          {(field) => (
            <div>
              <CarImageUpload
                value={field.state.value}
                onChange={(file) => field.handleChange(file)}
                disabled={isLoading}
              />
            </div>
          )}
        </form.Field>

        <form.Field name="carMakeId">
          {(field) => (
            <Select
              label="Make"
              placeholder="Select make"
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const makeId = value ? Number(value) : 0
                field.handleChange(makeId)
                setSelectedMakeId(makeId || null)
              }}
              data={makes.map((make) => ({
                value: make.id.toString(),
                label: make.name,
              }))}
              searchable
              required
              error={field.state.meta.errors.join(', ')}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="carModelId">
          {(field) => (
            <Select
              label="Model"
              placeholder="Select model"
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const modelId = value ? Number(value) : 0
                field.handleChange(modelId)
                setSelectedModelId(modelId || null)
              }}
              data={models.map((model) => ({
                value: model.id.toString(),
                label: model.name,
              }))}
              searchable
              required
              disabled={!selectedMakeId || isLoading}
              error={field.state.meta.errors.join(', ')}
            />
          )}
        </form.Field>

        <form.Field name="carGenerationId">
          {(field) => (
            <Select
              label="Generation"
              placeholder="Select generation (optional)"
              value={field.state.value ? field.state.value.toString() : null}
              onChange={(value) => {
                const generationId = value ? Number(value) : undefined
                field.handleChange(generationId)
              }}
              data={generations.map((gen) => ({
                value: gen.id.toString(),
                label: gen.name,
              }))}
              searchable
              clearable
              disabled={!selectedModelId || isLoading}
            />
          )}
        </form.Field>

        <form.Field name="year">
          {(field) => (
            <NumberInput
              label="Year"
              placeholder="e.g., 2020"
              value={field.state.value}
              onChange={(value) => field.handleChange(Number(value))}
              min={1900}
              max={new Date().getFullYear() + 1}
              required
              error={field.state.meta.errors.join(', ')}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="nickname">
          {(field) => (
            <TextInput
              label="Nickname"
              placeholder="e.g., My Daily Driver"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="color">
          {(field) => (
            <TextInput
              label="Color"
              placeholder="e.g., Red"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="description">
          {(field) => (
            <Textarea
              label="Description"
              placeholder="Tell us about your car..."
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              minRows={3}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="isPrimary">
          {(field) => (
            <Checkbox
              label="Set as primary car"
              checked={field.state.value}
              onChange={(e) => field.handleChange(e.target.checked)}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <Group justify="flex-end" mt="xl">
          <Button variant="subtle" onClick={onCancel} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" loading={isLoading}>
            {isEditing ? 'Save Changes' : 'Add Car'}
          </Button>
        </Group>
      </Stack>
    </form>
  )
}
