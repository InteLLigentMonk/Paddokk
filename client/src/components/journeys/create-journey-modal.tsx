import { useMemo, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import {
  Badge,
  Button,
  Checkbox,
  Group,
  Modal,
  ScrollArea,
  Select,
  Stack,
  Text,
  TextInput,
} from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import { useForm } from "@tanstack/react-form";
import { useQueryClient } from "@tanstack/react-query";
import {
  closeCreateJourneyModal,
  journeysPageStore,
} from "@/lib/stores/journeys-page-store";
import { useCanAddJourney } from "@/hooks/use-can-add-journey";
import { useUserCarsInfinite } from "@/hooks/use-user-cars";
import { createJourneyFn } from "@/lib/api/user-journeys";
import { journeyKeys } from "@/lib/api/journeys.keys";
import { userJourneysUploadJourneyCoverImage } from "@/generated/api/user-journeys/user-journeys";
import { handleUploadError } from "@/lib/api/upload-error";
import { RichTextEditor } from "@/components/shared/rich-text-editor";
import { CoverImageDropzone } from "@/components/shared/cover-image-dropzone";
import { useNotifications } from "@/integrations/mantine";

const JOURNEY_CATEGORIES = [
  { value: "1", label: "Build & Mods" },
  { value: "2", label: "Restoration" },
  { value: "3", label: "Racing" },
  { value: "4", label: "Adventures" },
  { value: "5", label: "Ownership" },
];

function getCarLabel(car: {
  isCustomBuild: boolean;
  customBuildName?: string | null;
  carMakeName?: string | null;
  carModelName?: string | null;
  carYear?: number | string | null;
  carNickname?: string | null;
}): string {
  if (car.isCustomBuild) return car.customBuildName || "Custom Build";
  const parts = [car.carMakeName, car.carModelName, car.carYear]
    .filter(Boolean)
    .join(" ");
  return car.carNickname ? `${parts} (${car.carNickname})` : parts;
}

export function CreateJourneyModal() {
  const isOpen = useStore(
    journeysPageStore,
    (state) => state.modals.createJourney,
  );
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const notifications = useNotifications();
  const { currentCount, maxJourneys } = useCanAddJourney();

  const modalTitle = (
    <Group gap="xs" align="center">
      <Text fw={600}>New journey</Text>
      {maxJourneys != null && currentCount != null && (
        <Badge variant="light" size="sm">
          {currentCount}/{maxJourneys}
        </Badge>
      )}
    </Group>
  );

  const [description, setDescription] = useState("");
  const [coverFile, setCoverFile] = useState<File | null>(null);
  const [coverPreviewUrl, setCoverPreviewUrl] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { data: carsData } = useUserCarsInfinite(isOpen);

  const cars = carsData?.pages.flatMap((p) => p.items) ?? [];

  const carsSelectData = useMemo(
    () =>
      cars.map((car) => ({
        value: String(car.id),
        label: getCarLabel(car),
      })),
    [cars],
  );

  const primaryCarId = useMemo(
    () => cars.find((c) => c.isPrimary)?.id ?? cars[0]?.id,
    [cars],
  );

  const form = useForm({
    defaultValues: {
      title: "",
      category: "" as string,
      userCarId: "" as string,
      setAsDefaultActive: true,
    },
    onSubmit: async ({ value }) => {
      const effectiveCarId = Number(value.userCarId) || Number(primaryCarId);
      if (!effectiveCarId || !value.category) return;
      setIsSubmitting(true);
      try {
        const journey = await createJourneyFn({
          data: {
            title: value.title,
            description: description || null,
            category: Number(value.category),
            userCarId: effectiveCarId,
            setAsDefaultActive: value.setAsDefaultActive,
          },
        });

        if (coverFile) {
          await userJourneysUploadJourneyCoverImage(journey.id, {
            file: coverFile,
          });
        }

        journeyKeys.userJourneyListRoots.forEach((queryKey) =>
          queryClient.invalidateQueries({ queryKey }),
        );
        queryClient.invalidateQueries({ queryKey: journeyKeys.journeyLimits });
        if (value.setAsDefaultActive) {
          queryClient.invalidateQueries({
            queryKey: journeyKeys.defaultActiveJourney,
          });
        }
        notifications.success({ message: "Journey created!" });
        handleClose();
        navigate({
          to: "/users/$username/journeys/$slug",
          params: {
            username: journey.ownerUsername,
            slug: journey.slug,
          },
        });
      } catch (err) {
        handleUploadError(err, "Failed to create journey");
        setIsSubmitting(false);
      }
    },
  });

  const handleClose = () => {
    form.reset();
    setDescription("");
    if (coverPreviewUrl) URL.revokeObjectURL(coverPreviewUrl);
    setCoverFile(null);
    setCoverPreviewUrl(null);
    setIsSubmitting(false);
    closeCreateJourneyModal();
  };

  const handleCoverChange = (file: File | null, previewUrl: string | null) => {
    setCoverFile(file);
    setCoverPreviewUrl(previewUrl);
  };

  return (
    <Modal
      opened={isOpen}
      onClose={handleClose}
      title={modalTitle}
      centered
      size="lg"
      scrollAreaComponent={ScrollArea.Autosize}
    >
      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
        }}
      >
        <Stack gap="md">
          <CoverImageDropzone
            previewUrl={coverPreviewUrl}
            onChange={handleCoverChange}
          />

          <form.Field
            name="title"
            validators={{
              onChange: ({ value }) => {
                if (!value.trim()) return "Title is required";
                if (value.trim().length < 3)
                  return "Title must be at least 3 characters";
                return undefined;
              },
            }}
          >
            {(field) => (
              <TextInput
                label="Title"
                placeholder="Give your journey a name"
                value={field.state.value}
                onChange={(e) => field.handleChange(e.target.value)}
                error={
                  field.state.meta.isTouched
                    ? field.state.meta.errors.join(", ")
                    : undefined
                }
                required
              />
            )}
          </form.Field>

          <form.Field
            name="category"
            validators={{
              onChange: ({ value }) =>
                !value ? "Category is required" : undefined,
            }}
          >
            {(field) => (
              <Select
                label="Category"
                placeholder="Select category"
                data={JOURNEY_CATEGORIES}
                value={field.state.value || null}
                onChange={(value) => field.handleChange(value ?? "")}
                error={
                  field.state.meta.isTouched
                    ? field.state.meta.errors.join(", ")
                    : undefined
                }
                required
              />
            )}
          </form.Field>

          <form.Field
            name="userCarId"
            validators={{
              onChange: ({ value }) =>
                !value && !primaryCarId ? "Select a car" : undefined,
            }}
          >
            {(field) => (
              <Select
                label="Car"
                placeholder="Select car"
                data={carsSelectData}
                value={
                  field.state.value ||
                  (primaryCarId ? String(primaryCarId) : null)
                }
                onChange={(value) => field.handleChange(value ?? "")}
                error={
                  field.state.meta.isTouched
                    ? field.state.meta.errors.join(", ")
                    : undefined
                }
                disabled={cars.length === 0}
                required
              />
            )}
          </form.Field>

          {cars.length === 0 && (
            <Text size="sm" c="dimmed">
              You need to add a car before creating a journey.
            </Text>
          )}

          <form.Field name="setAsDefaultActive">
            {(field) => (
              <Checkbox
                label="Set as default active journey"
                checked={field.state.value}
                onChange={(e) => field.handleChange(e.currentTarget.checked)}
              />
            )}
          </form.Field>

          <div>
            <Text size="sm" fw={500} mb={4}>
              Description (optional)
            </Text>
            <RichTextEditor content={description} onChange={setDescription} />
          </div>

          <Group justify="flex-end" mt="sm">
            <Button
              variant="subtle"
              onClick={handleClose}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              loading={isSubmitting}
              disabled={cars.length === 0}
            >
              Create journey
            </Button>
          </Group>
        </Stack>
      </form>
    </Modal>
  );
}
