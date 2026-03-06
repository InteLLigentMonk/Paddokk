import {
  Badge,
  Group,
  Stack,
  Text,
  ActionIcon,
  Card,
  Image,
  Box,
} from "@mantine/core";
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone";
import { Upload, Image as ImageIcon, X, Trash2 } from "lucide-react";
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
  useSortable,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { useQuery } from "@tanstack/react-query";
import { limitsGetImageLimits } from "@/generated/api/limits/limits";
import { useNotifications } from "@/integrations/mantine";
import { CarImagePreview } from "./car-image-preview";
import type { PendingImage } from "./car-form-stepper";
import type { CarImageDto } from "@/generated/api/schemas";

interface ExistingImageCardProps {
  image: CarImageDto;
  isPrimary: boolean;
  onDelete: () => void;
  onSetPrimary: () => void;
  id: string;
}

function ExistingImageCard({
  image,
  isPrimary,
  onDelete,
  onSetPrimary,
  id,
}: ExistingImageCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id,
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition: transition || "transform 0.2s, box-shadow 0.2s",
    opacity: isDragging ? 0.5 : 1,
    cursor: isDragging ? "grabbing" : "grab",
    position: "relative" as const,
  };

  const w = isPrimary
    ? { base: 160, sm: 180, md: 200 }
    : { base: 150, sm: 150, md: 150 };

  return (
    <Card
      ref={setNodeRef}
      {...attributes}
      shadow={isDragging ? "xl" : "sm"}
      padding={0}
      radius="md"
      withBorder
      onClick={onSetPrimary}
      style={{
        ...style,
        borderColor: isPrimary
          ? "var(--mantine-primary-color-filled)"
          : undefined,
        borderWidth: isPrimary ? "2px" : "1px",
      }}
      w={w}
      h={w}
    >
      {isPrimary && (
        <Badge
          variant="filled"
          color="blue"
          size="sm"
          style={{ position: "absolute", top: 8, right: 8, zIndex: 10 }}
        >
          Primary
        </Badge>
      )}

      <Box
        {...listeners}
        style={{ position: "absolute", inset: 0, padding: "4px" }}
      >
        <Image
          src={image.mediumUrl || image.imageUrl}
          alt={image.caption ?? "Car image"}
          fit="cover"
          w="100%"
          h="100%"
          radius="sm"
          draggable={false}
          onDragStart={(e) => e.preventDefault()}
        />
      </Box>

      <ActionIcon
        color="red"
        variant="filled"
        size="sm"
        style={{ position: "absolute", bottom: 8, right: 8, zIndex: 10 }}
        onPointerDown={(e) => e.stopPropagation()}
        onClick={(e) => {
          e.stopPropagation();
          onDelete();
        }}
      >
        <Trash2 size={16} />
      </ActionIcon>
    </Card>
  );
}

export interface EditCarImagesSectionProps {
  existingImages: CarImageDto[];
  pendingImages: PendingImage[];
  deletedImageIds: number[];
  primaryId:
    | { type: "existing"; id: number }
    | { type: "pending"; localId: string }
    | null;
  onPendingChange: (imgs: PendingImage[]) => void;
  onDeleteExisting: (id: number) => void;
  onReorderExisting: (ids: number[]) => void;
  onSetPrimary: (
    id: { type: "existing"; id: number } | { type: "pending"; localId: string },
  ) => void;
  isSubmitting: boolean;
}

export function EditCarImagesSection({
  existingImages,
  pendingImages,
  deletedImageIds,
  primaryId,
  onPendingChange,
  onDeleteExisting,
  onReorderExisting,
  onSetPrimary,
  isSubmitting,
}: EditCarImagesSectionProps) {
  const notifications = useNotifications();

  const { data: limitsData } = useQuery({
    queryKey: ["image-limits"],
    queryFn: () => limitsGetImageLimits(),
  });
  const maxImages =
    limitsData?.status === 200 ? Number(limitsData.data.maxImagesPerCar) : 10;

  const visibleExisting = existingImages.filter(
    (img) => !deletedImageIds.includes(Number(img.id)),
  );
  const totalImages = visibleExisting.length + pendingImages.length;
  const canUploadMore = totalImages < maxImages;

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  const handleFileDrop = (files: File[]) => {
    const newImages: PendingImage[] = [];
    for (const file of files) {
      if (totalImages + newImages.length >= maxImages) {
        notifications.warning({
          message: `Subscription limit reached (${maxImages}/${maxImages})`,
        });
        break;
      }
      newImages.push({
        localId: crypto.randomUUID(),
        file,
        previewUrl: URL.createObjectURL(file),
        isPrimary: false,
      });
    }
    if (newImages.length > 0) {
      onPendingChange([...pendingImages, ...newImages]);
    }
  };

  const handleDeletePending = (localId: string) => {
    const img = pendingImages.find((i) => i.localId === localId);
    if (img) URL.revokeObjectURL(img.previewUrl);
    onPendingChange(pendingImages.filter((i) => i.localId !== localId));
  };

  const handleExistingDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const oldIndex = visibleExisting.findIndex(
      (i) => String(i.id) === active.id,
    );
    const newIndex = visibleExisting.findIndex((i) => String(i.id) === over.id);
    if (oldIndex === -1 || newIndex === -1) return;
    const reordered = arrayMove(visibleExisting, oldIndex, newIndex);
    onReorderExisting(reordered.map((i) => Number(i.id)));
  };

  const handlePendingDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const oldIndex = pendingImages.findIndex((i) => i.localId === active.id);
    const newIndex = pendingImages.findIndex((i) => i.localId === over.id);
    if (oldIndex === -1 || newIndex === -1) return;
    onPendingChange(arrayMove(pendingImages, oldIndex, newIndex));
  };

  return (
    <Stack gap="md">
      <Group justify="space-between">
        <Text size="sm" fw={500}>
          Car Photos
        </Text>
        <Badge variant="light" size="sm">
          {totalImages} / {maxImages} images
        </Badge>
      </Group>

      <Dropzone
        onDrop={handleFileDrop}
        accept={IMAGE_MIME_TYPE}
        maxSize={5 * 1024 * 1024}
        multiple
        disabled={!canUploadMore || isSubmitting}
        style={{
          borderStyle: "dashed",
          borderWidth: "2px",
          borderRadius: "var(--mantine-radius-md)",
        }}
      >
        <Group
          justify="center"
          gap="xl"
          mih={120}
          style={{ pointerEvents: "none" }}
        >
          <Dropzone.Accept>
            <Upload size={40} color="var(--mantine-color-blue-6)" />
          </Dropzone.Accept>
          <Dropzone.Reject>
            <X size={40} color="var(--mantine-color-red-6)" />
          </Dropzone.Reject>
          <Dropzone.Idle>
            <ImageIcon size={40} opacity={canUploadMore ? 1 : 0.3} />
          </Dropzone.Idle>
          <div>
            <Text size="lg" inline>
              {canUploadMore
                ? "Drag images here or click to select"
                : "Upload limit reached"}
            </Text>
            <Text size="sm" c="dimmed" inline mt={7}>
              {canUploadMore ? "Max 5MB each" : "Upgrade to add more photos"}
            </Text>
          </div>
        </Group>
      </Dropzone>

      {visibleExisting.length > 0 && (
        <>
          <Text size="xs" c="dimmed" fw={500}>
            Existing photos (click to set primary)
          </Text>
          <DndContext
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handleExistingDragEnd}
          >
            <SortableContext
              items={visibleExisting.map((img) => String(img.id))}
              strategy={rectSortingStrategy}
            >
              <Group justify="center" gap="md" wrap="wrap">
                {visibleExisting.map((image) => {
                  const isSelected =
                    primaryId?.type === "existing" &&
                    primaryId.id === Number(image.id);
                  return (
                    <ExistingImageCard
                      key={String(image.id)}
                      image={image}
                      isPrimary={isSelected}
                      id={String(image.id)}
                      onSetPrimary={() =>
                        onSetPrimary({ type: "existing", id: Number(image.id) })
                      }
                      onDelete={() => onDeleteExisting(Number(image.id))}
                    />
                  );
                })}
              </Group>
            </SortableContext>
          </DndContext>
        </>
      )}

      {pendingImages.length > 0 && (
        <>
          <Text size="xs" c="dimmed" fw={500}>
            New photos (click to set primary)
          </Text>
          <DndContext
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handlePendingDragEnd}
          >
            <SortableContext
              items={pendingImages.map((img) => img.localId)}
              strategy={rectSortingStrategy}
            >
              <Group justify="center" gap="md" wrap="wrap">
                {pendingImages.map((image, index) => {
                  const isSelected =
                    primaryId?.type === "pending" &&
                    primaryId.localId === image.localId;
                  return (
                    <CarImagePreview
                      key={image.localId}
                      image={image}
                      isPrimary={isSelected}
                      onDelete={() => handleDeletePending(image.localId)}
                      onSetPrimary={() =>
                        onSetPrimary({
                          type: "pending",
                          localId: image.localId,
                        })
                      }
                      id={image.localId}
                      index={index}
                    />
                  );
                })}
              </Group>
            </SortableContext>
          </DndContext>
        </>
      )}
    </Stack>
  );
}
