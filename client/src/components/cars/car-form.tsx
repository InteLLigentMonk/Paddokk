import { useForm } from "@tanstack/react-form";
import { z } from "zod";
import {
  Stack,
  TextInput,
  NumberInput,
  Textarea,
  Button,
  Group,
  Select,
  Checkbox,
  type ComboboxItem,
} from "@mantine/core";
import { useState, useEffect, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  carsGetCarMakes,
  carsGetCarModels,
  carsGetCarGenerations,
} from "@/generated/api/cars/cars";
import {
  userCarsCreateUserCar,
  userCarsUpdateUserCar,
} from "@/generated/api/user-cars/user-cars";
import type { CreateUserCarCommand, UserCarDto } from "@/generated/api/schemas";
import { useNotifications } from "@/integrations/mantine";
import { CarImageUpload } from "./car-image-upload";

const carFormSchema = z.object({
  carMakeId: z.number().min(1, "Please select a make"),
  carModelId: z.number().min(1, "Please select a model"),
  carGenerationId: z.number().optional(),
  year: z
    .number()
    .min(1900, "Year must be 1900 or later")
    .max(new Date().getFullYear() + 1, "Invalid year"),
  nickname: z.string().optional(),
  color: z.string().optional(),
  description: z.string().optional(),
  isPrimary: z.boolean().optional(),
  primaryImage: z.instanceof(File).optional().or(z.string().optional()),
});

type CarFormValues = z.infer<typeof carFormSchema>;

interface CarFormProps {
  initialValues?: UserCarDto;
  carId?: number;
  onSuccess: () => void;
  onCancel: () => void;
}

export function CarForm({
  initialValues,
  carId,
  onSuccess,
  onCancel,
}: CarFormProps) {
  const isEditing = !!carId;
  const queryClient = useQueryClient();
  const notifications = useNotifications();

  const [selectedMakeId, setSelectedMakeId] = useState<number | null>(
    initialValues?.carMakeId ? Number(initialValues.carMakeId) : null,
  );
  const [selectedModelId, setSelectedModelId] = useState<number | null>(
    initialValues?.carModelId ? Number(initialValues.carModelId) : null,
  );

  const { data: makesData } = useQuery({
    queryKey: ["car-makes"],
    queryFn: () => carsGetCarMakes(),
  });
  const { data: modelsData } = useQuery({
    queryKey: ["car-models", selectedMakeId],
    queryFn: () => carsGetCarModels(selectedMakeId!),
    enabled: !!selectedMakeId,
  });
  const { data: generationsData } = useQuery({
    queryKey: ["car-generations", selectedModelId],
    queryFn: () => carsGetCarGenerations(selectedModelId!),
    enabled: !!selectedModelId,
  });

  const makes = makesData?.status === 200 ? makesData.data.makes : [];
  const models = modelsData?.status === 200 ? modelsData.data.models : [];
  const generations =
    generationsData?.status === 200 ? generationsData.data.generations : [];

  const makesSelectData: ComboboxItem[] = useMemo(
    () =>
      makes.map((make) => ({ value: make.id.toString(), label: make.name })),
    [makes],
  );
  const modelsSelectData: ComboboxItem[] = useMemo(
    () =>
      models.map((model) => ({
        value: model.id.toString(),
        label: model.name,
      })),
    [models],
  );
  const generationsSelectData: ComboboxItem[] = useMemo(
    () =>
      generations.map((gen) => ({ value: gen.id.toString(), label: gen.name })),
    [generations],
  );

  const addMutation = useMutation({
    mutationFn: (payload: Omit<CreateUserCarCommand, "subscriptionTier">) =>
      userCarsCreateUserCar(payload as CreateUserCarCommand),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-cars"] });
      queryClient.invalidateQueries({ queryKey: ["car-limits"] });
      notifications.success({ message: "Car added successfully!" });
      onSuccess();
    },
  });

  const editMutation = useMutation({
    mutationFn: ({
      id,
      nickname,
      color,
      description,
      isPrimary,
    }: {
      id: number;
      nickname: string | null;
      color: string | null;
      description: string | null;
      isPrimary: boolean | null;
    }) =>
      userCarsUpdateUserCar(id, {
        carId: id,
        nickname,
        color,
        description,
        isPrimary,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-cars"] });
      queryClient.invalidateQueries({ queryKey: ["car-limits"] });
      notifications.success({ message: "Car updated successfully!" });
      onSuccess();
    },
  });

  const form = useForm({
    defaultValues: {
      carMakeId: initialValues?.carMakeId ? Number(initialValues.carMakeId) : 0,
      carModelId: initialValues?.carModelId
        ? Number(initialValues.carModelId)
        : 0,
      carGenerationId: initialValues?.carGenerationId
        ? Number(initialValues.carGenerationId)
        : undefined,
      year: initialValues?.year
        ? Number(initialValues.year)
        : new Date().getFullYear(),
      nickname: initialValues?.nickname ?? "",
      color: initialValues?.color ?? "",
      description: initialValues?.description ?? "",
      isPrimary: initialValues?.isPrimary ?? false,
      primaryImage:
        (initialValues?.primaryImageUrl as File | string | undefined) ??
        undefined,
    } as CarFormValues,
    onSubmit: async ({ value }) => {
      if (isEditing) {
        await editMutation.mutateAsync({
          id: carId!,
          nickname: value.nickname || null,
          color: value.color || null,
          description: value.description || null,
          isPrimary: value.isPrimary ?? null,
        });
      } else {
        await addMutation.mutateAsync({
          carMakeId: value.carMakeId,
          carModelId: value.carModelId,
          carGenerationId: value.carGenerationId ?? null,
          year: value.year,
          nickname: value.nickname || null,
          color: value.color || null,
          description: value.description || null,
          isPrimary: value.isPrimary,
        });
      }
    },
  });

  const isLoading = addMutation.isPending || editMutation.isPending;

  useEffect(() => {
    const currentMakeId = form.state.values.carMakeId;
    if (selectedMakeId !== currentMakeId && !isEditing && currentMakeId !== 0) {
      form.setFieldValue("carModelId", 0);
      form.setFieldValue("carGenerationId", undefined);
      setSelectedModelId(null);
    }
  }, [selectedMakeId, form.state.values.carMakeId, isEditing, form]);

  useEffect(() => {
    const currentModelId = form.state.values.carModelId;
    if (
      selectedModelId !== currentModelId &&
      !isEditing &&
      currentModelId !== 0
    ) {
      form.setFieldValue("carGenerationId", undefined);
    }
  }, [selectedModelId, form.state.values.carModelId, isEditing, form]);

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        form.handleSubmit();
      }}
    >
      <Stack gap="md">
        <form.Field name="primaryImage">
          {(field) => (
            <div>
              <CarImageUpload
                value={field.state.value as File | string | undefined}
                onChange={(file) => field.handleChange(file ?? undefined)}
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
              value={
                field.state.value && field.state.value !== 0
                  ? field.state.value.toString()
                  : null
              }
              onChange={(value) => {
                const makeId = value ? Number(value) : 0;
                field.handleChange(makeId);
                setSelectedMakeId(makeId || null);
              }}
              onBlur={field.handleBlur}
              data={makesSelectData}
              searchable
              required
              error={field.state.meta.errors.join(", ")}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <form.Field name="carModelId">
          {(field) => (
            <Select
              label="Model"
              placeholder="Select model"
              value={
                field.state.value && field.state.value !== 0
                  ? field.state.value.toString()
                  : null
              }
              onChange={(value) => {
                const modelId = value ? Number(value) : 0;
                field.handleChange(modelId);
                setSelectedModelId(modelId || null);
              }}
              onBlur={field.handleBlur}
              data={modelsSelectData}
              searchable
              required
              disabled={!selectedMakeId || isLoading}
              error={field.state.meta.errors.join(", ")}
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
                field.handleChange(value ? Number(value) : undefined);
              }}
              onBlur={field.handleBlur}
              data={generationsSelectData}
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
              onBlur={field.handleBlur}
              min={1900}
              max={new Date().getFullYear() + 1}
              required
              error={field.state.meta.errors.join(", ")}
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
              onBlur={field.handleBlur}
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
              onBlur={field.handleBlur}
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
              onBlur={field.handleBlur}
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
              onBlur={field.handleBlur}
              disabled={isLoading}
            />
          )}
        </form.Field>

        <Group justify="flex-end" mt="xl">
          <Button variant="subtle" onClick={onCancel} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" loading={isLoading}>
            {isEditing ? "Save Changes" : "Add Car"}
          </Button>
        </Group>
      </Stack>
    </form>
  );
}
