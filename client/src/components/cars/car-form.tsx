import { useForm } from "@tanstack/react-form";
import {
  Button,
  Checkbox,
  Group,
  NumberInput,
  Select,
  Stack,
  Switch,
  TextInput,
} from "@mantine/core";
import { useEffect, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { CarImageUpload } from "./car-image-upload";
import type { ComboboxItem } from "@mantine/core";
import type {
  CreateUserCarCommand,
  UpdateUserCarCommand,
  UserCarDto,
} from "@/generated/api/schemas";
import {
  carsGetCarGenerations,
  carsGetCarMakes,
  carsGetCarModels,
} from "@/generated/api/cars/cars";
import {
  userCarsCreateUserCar,
  userCarsUpdateUserCar,
} from "@/generated/api/user-cars/user-cars";
import { useNotifications } from "@/integrations/mantine";

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

  const [isCustomBuild, setIsCustomBuild] = useState(
    initialValues?.isCustomBuild ?? false,
  );
  const [selectedMakeId, setSelectedMakeId] = useState<number | null>(
    initialValues?.carMakeId ? Number(initialValues.carMakeId) : null,
  );
  const [selectedModelId, setSelectedModelId] = useState<number | null>(
    initialValues?.carModelId ? Number(initialValues.carModelId) : null,
  );

  const { data: makesData } = useQuery({
    queryKey: ["car-makes"],
    queryFn: () => carsGetCarMakes(),
    enabled: !isCustomBuild,
  });
  const { data: modelsData } = useQuery({
    queryKey: ["car-models", selectedMakeId],
    queryFn: () => carsGetCarModels(selectedMakeId!),
    enabled: !isCustomBuild && !!selectedMakeId,
  });
  const { data: generationsData } = useQuery({
    queryKey: ["car-generations", selectedModelId],
    queryFn: () => carsGetCarGenerations(selectedModelId!),
    enabled: !isCustomBuild && !!selectedModelId,
  });

  const makes = makesData?.status === 200 ? makesData.data.makes : [];
  const models = modelsData?.status === 200 ? modelsData.data.models : [];
  const generations =
    generationsData?.status === 200 ? generationsData.data.generations : [];

  const makesSelectData: Array<ComboboxItem> = useMemo(
    () =>
      makes.map((make) => ({ value: make.id.toString(), label: make.name })),
    [makes],
  );
  const modelsSelectData: Array<ComboboxItem> = useMemo(
    () =>
      models.map((model) => ({
        value: model.id.toString(),
        label: model.name,
      })),
    [models],
  );
  const generationsSelectData: Array<ComboboxItem> = useMemo(
    () =>
      generations.map((gen) => ({ value: gen.id.toString(), label: gen.name })),
    [generations],
  );

  const addMutation = useMutation({
    mutationFn: (payload: CreateUserCarCommand) =>
      userCarsCreateUserCar(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({
        predicate: (q) => {
          const key = q.queryKey[0];
          return key === "user-cars" || key === "user-cars-by-username";
        },
      });
      queryClient.invalidateQueries({ queryKey: ["car-limits"] });
      notifications.success({ message: "Car added successfully!" });
      onSuccess();
    },
  });

  const editMutation = useMutation({
    mutationFn: ({
      id,
      customBuildName,
      nickname,
      color,
      isPrimary,
    }: {
      id: number;
      customBuildName: string;
      nickname: string;
      color: string;
      isPrimary: boolean | null;
    }) =>
      userCarsUpdateUserCar(id, {
        carId: id,
        customBuildName,
        nickname,
        color,
        isPrimary,
      } as UpdateUserCarCommand),
    onSuccess: () => {
      queryClient.invalidateQueries({
        predicate: (q) => {
          const key = q.queryKey[0];
          return key === "user-cars" || key === "user-cars-by-username";
        },
      });
      queryClient.invalidateQueries({ queryKey: ["car-limits"] });
      notifications.success({ message: "Car updated successfully!" });
      onSuccess();
    },
  });

  const form = useForm({
    defaultValues: {
      customBuildName: initialValues?.customBuildName ?? "",
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
      isPrimary: initialValues?.isPrimary ?? false,
      primaryImage:
        (initialValues?.primaryImageUrl as File | string | undefined) ??
        undefined,
    },
    onSubmit: async ({ value }) => {
      if (isEditing) {
        await editMutation.mutateAsync({
          id: carId,
          customBuildName: value.customBuildName,
          nickname: value.nickname,
          color: value.color,
          isPrimary: value.isPrimary,
        });
      } else {
        await addMutation.mutateAsync({
          isCustomBuild,
          customBuildName: isCustomBuild ? value.customBuildName || null : null,
          carMakeId: isCustomBuild ? null : value.carMakeId,
          carModelId: isCustomBuild ? null : value.carModelId,
          carGenerationId: isCustomBuild
            ? null
            : (value.carGenerationId ?? null),
          year: isCustomBuild ? null : value.year,
          nickname: value.nickname || null,
          color: value.color || null,
          isPrimary: value.isPrimary,
        } as CreateUserCarCommand);
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

        {!isEditing && (
          <Switch
            label="Custom build"
            description="Enable if your car doesn't fit a standard make/model"
            checked={isCustomBuild}
            onChange={(e) => setIsCustomBuild(e.currentTarget.checked)}
            disabled={isLoading}
          />
        )}

        {isCustomBuild && !isEditing ? (
          <form.Field name="customBuildName">
            {(field) => (
              <TextInput
                label="Build name"
                placeholder="e.g. SR20DET S13, AE86 with 4AGE, custom turbo build"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                required
                disabled={isLoading}
              />
            )}
          </form.Field>
        ) : (
          !isEditing && (
            <>
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
                    value={
                      field.state.value ? field.state.value.toString() : null
                    }
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
            </>
          )
        )}

        {isEditing && initialValues?.isCustomBuild && (
          <form.Field name="customBuildName">
            {(field) => (
              <TextInput
                label="Build name"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                onBlur={field.handleBlur}
                disabled={isLoading}
              />
            )}
          </form.Field>
        )}

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
