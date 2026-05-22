import { ActionIcon, Badge, Box, Card, Image } from "@mantine/core";
import { GripVertical, Trash2 } from "lucide-react";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import type { PendingImage } from "./car-form-stepper";

interface CarImagePreviewProps {
  image: PendingImage;
  isPrimary: boolean;
  onDelete: () => void;
  onSetPrimary: () => void;
  id: string;
  index: number;
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
      w={
        isPrimary
          ? { base: 160, sm: 180, md: 200 }
          : { base: 150, sm: 150, md: 150 }
      }
      h={
        isPrimary
          ? { base: 160, sm: 180, md: 200 }
          : { base: 150, sm: 150, md: 150 }
      }
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

      <Box style={{ position: "absolute", inset: 0, padding: "4px" }}>
        <Image
          src={image.previewUrl}
          alt={image.file.name}
          fit="cover"
          w="100%"
          h="100%"
          radius="sm"
          draggable={false}
          onDragStart={(e) => e.preventDefault()}
        />
      </Box>

      <ActionIcon
        {...listeners}
        variant="filled"
        color="dark"
        size="sm"
        style={{
          position: "absolute",
          top: 8,
          left: 8,
          zIndex: 10,
          touchAction: "none",
          cursor: isDragging ? "grabbing" : "grab",
        }}
        onPointerDown={(e) => e.stopPropagation()}
      >
        <GripVertical size={14} />
      </ActionIcon>

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
