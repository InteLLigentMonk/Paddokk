import { useState, useEffect } from "react";
import {
  Button,
  Group,
  Stack,
  Text,
  Badge,
  Loader,
  Center,
} from "@mantine/core";
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone";
import { Upload, Image as ImageIcon, X } from "lucide-react";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  rectSortingStrategy,
} from "@dnd-kit/sortable";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { limitsGetImageLimits } from "@/generated/api/limits/limits";
import {
  carImagesGetCarImages,
  carImagesUploadCarImage,
  carImagesDeleteCarImage,
  carImagesUpdateCarImage,
  carImagesSetPrimaryImage,
} from "@/generated/api/car-images/car-images";
import type {
  CarImageDto,
  UpdateCarImageRequest,
} from "@/generated/api/schemas";
import { useNotifications } from "@/integrations/mantine";
import { CarImagePreview } from "./car-image-preview";

interface CarImagesStepProps {
  carId: number;
  onFinish: () => void;
  onBack: () => void;
}

export function CarImagesStep({ carId, onFinish, onBack }: CarImagesStepProps) {
  const notifications = useNotifications();
  const queryClient = useQueryClient();

  const { data: limitsData } = useQuery({
    queryKey: ["image-limits"],
    queryFn: () => limitsGetImageLimits(),
  });
  const maxImages =
    limitsData?.status === 200 ? Number(limitsData.data.maxImagesPerCar) : 10;

  const { data: existingImagesData } = useQuery({
    queryKey: ["car-images", carId],
    queryFn: () => carImagesGetCarImages(carId),
  });

  const [uploadedImages, setUploadedImages] = useState<CarImageDto[]>([]);

  useEffect(() => {
    if (existingImagesData?.status === 200) {
      setUploadedImages(existingImagesData.data.images);
    }
  }, [existingImagesData]);
  const [isUploading, setIsUploading] = useState(false);

  const uploadMutation = useMutation({
    mutationFn: (file: File) => carImagesUploadCarImage(carId, { File: file }),
  });

  const deleteMutation = useMutation({
    mutationFn: (imageId: number | string) =>
      carImagesDeleteCarImage(carId, imageId),
  });

  const updateMutation = useMutation({
    mutationFn: ({
      imageId,
      data,
    }: {
      imageId: number | string;
      data: UpdateCarImageRequest;
    }) => carImagesUpdateCarImage(String(carId), imageId, data),
  });

  const setPrimaryMutation = useMutation({
    mutationFn: (imageId: number | string) =>
      carImagesSetPrimaryImage(carId, imageId),
  });

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  const handleFileDrop = async (files: File[]) => {
    setIsUploading(true);
    let successCount = 0;

    for (const file of files) {
      if (uploadedImages.length + successCount >= maxImages) {
        notifications.warning({
          message: `Subscription limit reached (${maxImages}/${maxImages})`,
        });
        break;
      }

      try {
        const result = await uploadMutation.mutateAsync(file);
        const newImage = result.data as CarImageDto;
        setUploadedImages((prev) => [...prev, newImage]);
        successCount++;

        if (
          uploadedImages.length === 0 &&
          successCount === 1 &&
          newImage.id != null
        ) {
          await setPrimaryMutation.mutateAsync(newImage.id);
          setUploadedImages((prev) =>
            prev.map((img) => ({ ...img, isPrimary: img.id === newImage.id })),
          );
        }
      } catch {
        notifications.error({ message: `Failed to upload ${file.name}` });
      }
    }

    setIsUploading(false);
    if (successCount > 0) {
      notifications.success({
        message: `${successCount} image${successCount > 1 ? "s" : ""} uploaded successfully!`,
      });
    }
  };

  const handleDelete = async (imageId: number | string) => {
    try {
      const deletedImage = uploadedImages.find((img) => img.id === imageId);
      await deleteMutation.mutateAsync(imageId);
      setUploadedImages((prev) => prev.filter((img) => img.id !== imageId));

      if (deletedImage?.isPrimary && uploadedImages.length > 1) {
        const newPrimary = uploadedImages.find((img) => img.id !== imageId);
        if (newPrimary?.id != null) {
          await setPrimaryMutation.mutateAsync(newPrimary.id);
          setUploadedImages((prev) =>
            prev.map((img) => ({
              ...img,
              isPrimary: img.id === newPrimary.id,
            })),
          );
        }
      }
    } catch {
      notifications.error({ message: "Failed to delete image" });
    }
  };

  const handleSetPrimary = async (imageId: number | string) => {
    try {
      await setPrimaryMutation.mutateAsync(imageId);
      setUploadedImages((prev) =>
        prev.map((img) => ({ ...img, isPrimary: img.id === imageId })),
      );
    } catch {
      notifications.error({ message: "Failed to set primary image" });
    }
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const oldIndex = uploadedImages.findIndex(
      (img) => img.id === Number(active.id),
    );
    const newIndex = uploadedImages.findIndex(
      (img) => img.id === Number(over.id),
    );
    if (oldIndex === -1 || newIndex === -1) return;

    const oldImages = uploadedImages;
    const reordered = arrayMove(uploadedImages, oldIndex, newIndex);
    setUploadedImages(reordered);

    try {
      await Promise.all(
        reordered.map((img, index) =>
          img.id != null
            ? updateMutation.mutateAsync({
                imageId: img.id,
                data: { sortOrder: index },
              })
            : Promise.resolve(),
        ),
      );
    } catch {
      setUploadedImages(oldImages);
      notifications.error({ message: "Failed to reorder images" });
    }
  };

  const handleFinish = () => {
    queryClient.invalidateQueries({ queryKey: ["user-cars"] });
    notifications.success({ message: "Car added successfully!" });
    onFinish();
  };

  const canUploadMore = uploadedImages.length < maxImages;

  return (
    <Stack gap="md" mt="md">
      <div>
        <Group justify="space-between" mb="sm">
          <Text size="sm" fw={500}>
            Car Photos
          </Text>
          <Badge variant="light" size="sm">
            {uploadedImages.length} / {maxImages} images
          </Badge>
        </Group>

        <Dropzone
          onDrop={handleFileDrop}
          accept={IMAGE_MIME_TYPE}
          maxSize={5 * 1024 * 1024}
          multiple
          disabled={!canUploadMore || isUploading}
        >
          <Group
            justify="center"
            gap="xl"
            mih={120}
            style={{ pointerEvents: "none" }}
          >
            <Dropzone.Accept>
              <Upload size={52} color="var(--mantine-color-blue-6)" />
            </Dropzone.Accept>
            <Dropzone.Reject>
              <X size={52} color="var(--mantine-color-red-6)" />
            </Dropzone.Reject>
            <Dropzone.Idle>
              {isUploading ? (
                <Loader size={52} />
              ) : (
                <ImageIcon size={52} opacity={canUploadMore ? 1 : 0.3} />
              )}
            </Dropzone.Idle>

            <div>
              <Text size="xl" inline>
                {isUploading
                  ? "Uploading..."
                  : canUploadMore
                    ? "Drag images here or click to select"
                    : "Upload limit reached"}
              </Text>
              <Text size="sm" c="dimmed" inline mt={7}>
                {canUploadMore
                  ? "Upload multiple files (max 5MB each)"
                  : maxImages === 0
                    ? "Upgrade your plan to add photos"
                    : "Upgrade to add more photos"}
              </Text>
            </div>
          </Group>
        </Dropzone>
      </div>

      {uploadedImages.length > 0 && (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext
            items={uploadedImages.map((img) => Number(img.id))}
            strategy={rectSortingStrategy}
          >
            <Group justify="center" gap="md" wrap="wrap">
              {uploadedImages.map((image) => (
                <CarImagePreview
                  key={image.id}
                  image={image}
                  isPrimary={image.isPrimary ?? false}
                  onDelete={() => image.id != null && handleDelete(image.id)}
                  onSetPrimary={() =>
                    image.id != null && handleSetPrimary(image.id)
                  }
                  id={Number(image.id)}
                  index={Number(image.sortOrder)}
                />
              ))}
            </Group>
          </SortableContext>
        </DndContext>
      )}

      {uploadedImages.length === 0 && !isUploading && (
        <Center py="xl">
          <Stack gap="xs" align="center">
            <ImageIcon size={48} opacity={0.3} />
            <Text size="sm" c="dimmed" ta="center">
              No photos yet. {canUploadMore ? "Drag and drop to upload." : ""}
            </Text>
            {!canUploadMore && maxImages === 0 && (
              <Text size="xs" c="dimmed" ta="center">
                Upgrade your plan to add photos to your cars.
              </Text>
            )}
          </Stack>
        </Center>
      )}

      {isUploading && uploadedImages.length === 0 && (
        <Center py="xl">
          <Stack gap="md" align="center">
            <Loader size="lg" />
            <Text size="sm" c="dimmed">
              Uploading images...
            </Text>
          </Stack>
        </Center>
      )}

      <Group justify="space-between" mt="md">
        <Button variant="subtle" onClick={onBack}>
          Back
        </Button>
        <Button onClick={handleFinish} loading={isUploading}>
          Finish
        </Button>
      </Group>
    </Stack>
  );
}
