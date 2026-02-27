import { ActionIcon, Badge, Box, Card, Image } from "@mantine/core"
import { Trash2, GripVertical } from "lucide-react"
import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import type { CarImageDto } from "@/generated/api/schemas"

interface CarImagePreviewProps {
  image: CarImageDto
  isPrimary: boolean
  onDelete: () => void
  onSetPrimary: () => void
  id: number
  index: number
}

export function CarImagePreview({
  image,
  isPrimary,
  onDelete,
  onSetPrimary,
  id,
}: CarImagePreviewProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition: transition || "transform 0.2s, box-shadow 0.2s",
    opacity: isDragging ? 0.5 : 1,
    cursor: "pointer",
    position: "relative" as const,
  }

  return (
    <Card
      ref={setNodeRef}
      shadow={isDragging ? "xl" : "sm"}
      padding="xs"
      radius="md"
      withBorder
      onClick={onSetPrimary}
      style={{
        ...style,
        borderColor: isPrimary ? "var(--mantine-primary-color-filled)" : undefined,
        borderWidth: isPrimary ? "2px" : "1px",
      }}
      // Responsive sizing via inline styles based on isPrimary
      w={isPrimary ? { base: 160, sm: 180, md: 200 } : { base: 150, sm: 150, md: 150 }}
      h={isPrimary ? { base: 160, sm: 180, md: 200 } : { base: 150, sm: 150, md: 150 }}
    >
      {/* Drag handle */}
      <Box
        {...attributes}
        {...listeners}
        style={{
          position: "absolute",
          top: 8,
          left: 8,
          zIndex: 10,
          cursor: isDragging ? "grabbing" : "grab",
        }}
        onClick={(e) => e.stopPropagation()} // Prevent triggering onSetPrimary
      >
        <GripVertical size={20} opacity={0.5} />
      </Box>

      {/* Primary badge */}
      {isPrimary && (
        <Badge
          variant="filled"
          color="blue"
          size="sm"
          style={{
            position: "absolute",
            top: 8,
            right: 8,
            zIndex: 10,
          }}
        >
          Primary
        </Badge>
      )}

      {/* Image thumbnail */}
      <Box
        style={{
          position: "absolute",
          inset: 0,
          padding: "var(--mantine-spacing-xs)",
        }}
      >
        <Image
          src={image.thumbnailUrl || image.imageUrl}
          alt={`Car image ${image.id}`}
          fit="cover"
          w="100%"
          h="100%"
          radius="sm"
        />
      </Box>

      {/* Delete button */}
      <ActionIcon
        color="red"
        variant="filled"
        size="sm"
        style={{
          position: "absolute",
          bottom: 8,
          right: 8,
          zIndex: 10,
        }}
        onClick={(e) => {
          e.stopPropagation() // Prevent triggering onSetPrimary
          onDelete()
        }}
      >
        <Trash2 size={16} />
      </ActionIcon>
    </Card>
  )
}
