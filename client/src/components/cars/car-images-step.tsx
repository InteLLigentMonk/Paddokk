import { useState } from "react"
import { Button, Group, Stack, Text, SimpleGrid, Badge, Loader, Center } from "@mantine/core"
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone"
import { Upload, Image as ImageIcon, X } from "lucide-react"
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core"
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  rectSortingStrategy,
} from "@dnd-kit/sortable"
import { useGetApiImagesLimits } from "@/generated/api/images/images"
import {
  usePostApiUsersMeCarsCarIdImages,
  useDeleteApiUsersMeCarsCarIdImagesImageId,
  usePutApiUsersMeCarsCarIdImagesImageId,
  usePutApiUsersMeCarsCarIdImagesImageIdSetprimary,
} from "@/generated/api/car-images/car-images"
import { getGetApiUsersMeCarsQueryKey } from "@/generated/api/user-cars/user-cars"
import type { CarImageDto } from "@/generated/api"
import { useNotifications } from "@/integrations/mantine"
import { useQueryClient } from "@tanstack/react-query"
import { CarImagePreview } from "./car-image-preview"

interface CarImagesStepProps {
  carId: number // From Step 1
  onFinish: () => void // Close modal
  onBack: () => void // Return to Step 1
}

export function CarImagesStep({ carId, onFinish, onBack }: CarImagesStepProps) {
  const notifications = useNotifications()
  const queryClient = useQueryClient()

  // Fetch image limits
  const { data: limitsData } = useGetApiImagesLimits()
  // Handle both direct DTO and {data, status} response formats
  const limits = limitsData && 'data' in limitsData ? limitsData.data : limitsData
  const maxImages = limits?.maxImagesPerCar ?? 10

  // TODO: State for uploaded images
  const [uploadedImages, setUploadedImages] = useState<CarImageDto[]>([])
  const [isUploading, setIsUploading] = useState(false)

  // TODO: Setup mutations
  const uploadImageMutation = usePostApiUsersMeCarsCarIdImages()
  const deleteImageMutation = useDeleteApiUsersMeCarsCarIdImagesImageId()
  const updateImageMutation = usePutApiUsersMeCarsCarIdImagesImageId()
  const setPrimaryMutation = usePutApiUsersMeCarsCarIdImagesImageIdSetprimary()

  // Setup drag-and-drop sensors
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  // TODO: Handle file drop (instant upload)
  const handleFileDrop = async (files: File[]) => {
    setIsUploading(true)
    let successCount = 0
    let failCount = 0

    for (const file of files) {
      if (uploadedImages.length + successCount >= maxImages) {
        notifications.warning({
          message: `Subscription limit reached (${maxImages}/${maxImages})`,
        })
        break
      }

      const formData = new FormData()
      formData.append("file", file)

      try {
        const newImage = await uploadImageMutation.mutateAsync({
          carId,
          data: formData,
        })

        setUploadedImages((prev) => [...prev, newImage])
        successCount++

        // If first image, auto-set as primary
        if (uploadedImages.length === 0 && successCount === 1) {
          await setPrimaryMutation.mutateAsync({
            carId,
            imageId: newImage.id,
          })
          // Update isPrimary flag in local state
          setUploadedImages((prev) =>
            prev.map((img) =>
              img.id === newImage.id ? { ...img, isPrimary: true } : img
            )
          )
        }
      } catch (error) {
        notifications.error({ message: `Failed to upload ${file.name}` })
        failCount++
      }
    }

    setIsUploading(false)

    // Show success notification
    if (successCount > 0) {
      notifications.success({
        message: `${successCount} image${successCount > 1 ? "s" : ""} uploaded successfully!`,
      })
    }
  }

  // TODO: Handle delete
  const handleDelete = async (imageId: number) => {
    try {
      await deleteImageMutation.mutateAsync({ carId, imageId })

      const deletedImage = uploadedImages.find((img) => img.id === imageId)
      setUploadedImages((prev) => prev.filter((img) => img.id !== imageId))

      // If deleted image was primary, set first remaining as primary
      if (deletedImage?.isPrimary && uploadedImages.length > 1) {
        const newPrimary = uploadedImages.find((img) => img.id !== imageId)
        if (newPrimary) {
          await setPrimaryMutation.mutateAsync({
            carId,
            imageId: newPrimary.id,
          })
          setUploadedImages((prev) =>
            prev.map((img) =>
              img.id === newPrimary.id ? { ...img, isPrimary: true } : img
            )
          )
        }
      }
    } catch (error) {
      notifications.error({ message: "Failed to delete image" })
    }
  }

  // TODO: Handle set primary
  const handleSetPrimary = async (imageId: number) => {
    try {
      await setPrimaryMutation.mutateAsync({ carId, imageId })

      // Update local state
      setUploadedImages((prev) =>
        prev.map((img) => ({ ...img, isPrimary: img.id === imageId }))
      )
    } catch (error) {
      notifications.error({ message: "Failed to set primary image" })
    }
  }

  // Handle drag end (reorder images)
  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return

    // Find indices
    const oldIndex = uploadedImages.findIndex((img) => img.id === Number(active.id))
    const newIndex = uploadedImages.findIndex((img) => img.id === Number(over.id))

    if (oldIndex === -1 || newIndex === -1) return

    // Optimistic update
    const oldImages = uploadedImages
    const reordered = arrayMove(uploadedImages, oldIndex, newIndex)
    setUploadedImages(reordered)

    try {
      // Update sortOrder for all affected images
      await Promise.all(
        reordered.map((img, index) =>
          updateImageMutation.mutateAsync({
            carId,
            imageId: img.id,
            data: { sortOrder: index },
          })
        )
      )
    } catch (error) {
      // Revert on error
      setUploadedImages(oldImages)
      notifications.error({ message: "Failed to reorder images" })
    }
  }

  // TODO: Handle finish
  const handleFinish = () => {
    queryClient.invalidateQueries({ queryKey: getGetApiUsersMeCarsQueryKey() })
    notifications.success({ message: "Car added successfully!" })
    onFinish()
  }

  const canUploadMore = uploadedImages.length < maxImages

  return (
    <Stack gap="md" mt="md">
      {/* TODO: Render Dropzone */}
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
          maxSize={5 * 1024 * 1024} // 5MB
          multiple
          disabled={!canUploadMore || isUploading}
        >
          <Group justify="center" gap="xl" mih={120} style={{ pointerEvents: "none" }}>
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

      {/* TODO: Render uploaded images grid with drag-and-drop */}
      {uploadedImages.length > 0 && (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext
            items={uploadedImages.map((img) => img.id)}
            strategy={rectSortingStrategy}
          >
            <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="md">
              {uploadedImages.map((image) => (
                <CarImagePreview
                  key={image.id}
                  image={image}
                  isPrimary={image.isPrimary}
                  onDelete={() => handleDelete(image.id)}
                  onSetPrimary={() => handleSetPrimary(image.id)}
                  id={image.id}
                  index={image.sortOrder}
                />
              ))}
            </SimpleGrid>
          </SortableContext>
        </DndContext>
      )}

      {/* Empty state */}
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

      {/* Loading state */}
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

      {/* TODO: Add Back and Finish buttons */}
      <Group justify="space-between" mt="md">
        <Button variant="subtle" onClick={onBack}>
          Back
        </Button>
        <Button onClick={handleFinish} loading={isUploading}>
          Finish
        </Button>
      </Group>
    </Stack>
  )
}
