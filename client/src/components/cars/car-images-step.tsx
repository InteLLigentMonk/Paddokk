import { Badge, Button, Group, Stack, Text } from "@mantine/core"
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone"
import { Image as ImageIcon, Upload, X } from "lucide-react"
import {
  DndContext,
  
  KeyboardSensor,
  PointerSensor,
  closestCenter,
  useSensor,
  useSensors
} from "@dnd-kit/core"
import {
  SortableContext,
  arrayMove,
  rectSortingStrategy,
  sortableKeyboardCoordinates,
} from "@dnd-kit/sortable"
import { useQuery } from "@tanstack/react-query"
import { CarImagePreview } from "./car-image-preview"
import type {DragEndEvent} from "@dnd-kit/core";
import type { PendingImage } from "./car-form-stepper"
import { limitsGetImageLimits } from "@/generated/api/limits/limits"
import { useNotifications } from "@/integrations/mantine"

interface CarImagesStepProps {
  pendingImages: Array<PendingImage>
  onImagesChange: (imgs: Array<PendingImage>) => void
  isSubmitting: boolean
  onFinish: () => void
  onBack: () => void
}

export function CarImagesStep({
  pendingImages,
  onImagesChange,
  isSubmitting,
  onFinish,
  onBack,
}: CarImagesStepProps) {
  const notifications = useNotifications()

  const { data: limitsData } = useQuery({
    queryKey: ["image-limits"],
    queryFn: () => limitsGetImageLimits(),
  })
  const maxImages = limitsData?.status === 200 ? Number(limitsData.data.maxImagesPerCar) : 10

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  )

  const handleFileDrop = (files: Array<File>) => {
    const newImages: Array<PendingImage> = []
    for (const file of files) {
      if (pendingImages.length + newImages.length >= maxImages) {
        notifications.warning({
          message: `Subscription limit reached (${maxImages}/${maxImages})`,
        })
        break
      }
      const isFirst = pendingImages.length === 0 && newImages.length === 0
      newImages.push({
        localId: crypto.randomUUID(),
        file,
        previewUrl: URL.createObjectURL(file),
        isPrimary: isFirst,
      })
    }
    if (newImages.length > 0) {
      onImagesChange([...pendingImages, ...newImages])
    }
  }

  const handleDelete = (localId: string) => {
    const img = pendingImages.find((i) => i.localId === localId)
    if (img) URL.revokeObjectURL(img.previewUrl)
    const remaining = pendingImages.filter((i) => i.localId !== localId)
    if (img?.isPrimary && remaining.length > 0) {
      onImagesChange(remaining.map((i, idx) => ({ ...i, isPrimary: idx === 0 })))
    } else {
      onImagesChange(remaining)
    }
  }

  const handleSetPrimary = (localId: string) => {
    onImagesChange(pendingImages.map((i) => ({ ...i, isPrimary: i.localId === localId })))
  }

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return
    const oldIndex = pendingImages.findIndex((i) => i.localId === active.id)
    const newIndex = pendingImages.findIndex((i) => i.localId === over.id)
    if (oldIndex === -1 || newIndex === -1) return
    onImagesChange(arrayMove(pendingImages, oldIndex, newIndex))
  }

  const canUploadMore = pendingImages.length < maxImages

  return (
    <Stack gap="md" mt="md">
      <div>
        <Group justify="space-between" mb="sm">
          <Text size="sm" fw={500}>
            Car Photos
          </Text>
          <Badge variant="light" size="sm">
            {pendingImages.length} / {maxImages} images
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
            mih={pendingImages.length === 0 ? 220 : 120}
            style={{ pointerEvents: "none" }}
          >
            <Dropzone.Accept>
              <Upload size={52} color="var(--mantine-color-blue-6)" />
            </Dropzone.Accept>
            <Dropzone.Reject>
              <X size={52} color="var(--mantine-color-red-6)" />
            </Dropzone.Reject>
            <Dropzone.Idle>
              <ImageIcon size={52} opacity={canUploadMore ? 1 : 0.3} />
            </Dropzone.Idle>

            <div>
              <Text size="xl" inline>
                {canUploadMore ? "Drag images here or click to select" : "Upload limit reached"}
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

      {pendingImages.length > 0 && (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext
            items={pendingImages.map((img) => img.localId)}
            strategy={rectSortingStrategy}
          >
            <Group justify="center" gap="md" wrap="wrap">
              {pendingImages.map((image, index) => (
                <CarImagePreview
                  key={image.localId}
                  image={image}
                  isPrimary={image.isPrimary}
                  onDelete={() => handleDelete(image.localId)}
                  onSetPrimary={() => handleSetPrimary(image.localId)}
                  id={image.localId}
                  index={index}
                />
              ))}
            </Group>
          </SortableContext>
        </DndContext>
      )}

      <Group justify="space-between" mt="md">
        <Button variant="subtle" onClick={onBack} disabled={isSubmitting}>
          Back
        </Button>
        <Button onClick={onFinish} loading={isSubmitting}>
          Finish
        </Button>
      </Group>
    </Stack>
  )
}
