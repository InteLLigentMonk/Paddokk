import {
  ActionIcon,
  AspectRatio,
  Box,
  Group,
  Image,
  Text,
} from "@mantine/core";
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone";
import { Image as ImageIcon, Upload, X } from "lucide-react";

interface CoverImageDropzoneProps {
  previewUrl: string | null;
  onChange: (file: File | null, previewUrl: string | null) => void;
}

export function CoverImageDropzone({
  previewUrl,
  onChange,
}: CoverImageDropzoneProps) {
  const handleDrop = (files: Array<File>) => {
    if (files.length === 0) return;
    const file = files[0];
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    onChange(file, URL.createObjectURL(file));
  };

  const handleRemove = () => {
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    onChange(null, null);
  };

  if (previewUrl) {
    return (
      <AspectRatio ratio={16 / 9}>
        <Box
          pos="relative"
          style={{
            borderRadius: "var(--mantine-radius-md)",
            overflow: "hidden",
            width: "100%",
            height: "100%",
          }}
        >
          <Image
            src={previewUrl}
            alt="Cover preview"
            fit="cover"
            style={{ width: "100%", height: "100%" }}
          />
          <ActionIcon
            pos="absolute"
            top={8}
            right={8}
            variant="filled"
            color="red"
            size="sm"
            onClick={handleRemove}
            aria-label="Remove cover image"
          >
            <X size={14} />
          </ActionIcon>
        </Box>
      </AspectRatio>
    );
  }

  return (
    <AspectRatio ratio={16 / 9}>
      <Dropzone
        onDrop={handleDrop}
        accept={IMAGE_MIME_TYPE}
        maxSize={5 * 1024 * 1024}
        multiple={false}
        style={{
          width: "100%",
          height: "100%",
          borderStyle: "dashed",
          borderWidth: "2px",
          borderRadius: "var(--mantine-radius-md)",
        }}
        styles={{ inner: { height: "100%" } }}
      >
        <Group
          justify="center"
          align="center"
          gap="xl"
          h="100%"
          style={{ pointerEvents: "none" }}
        >
          <Dropzone.Accept>
            <Upload size={40} color="var(--mantine-color-blue-6)" />
          </Dropzone.Accept>
          <Dropzone.Reject>
            <X size={40} color="var(--mantine-color-red-6)" />
          </Dropzone.Reject>
          <Dropzone.Idle>
            <ImageIcon size={40} opacity={0.5} />
          </Dropzone.Idle>
          <div>
            <Text size="md" inline>
              Drag cover image here or click to select
            </Text>
            <Text size="sm" c="dimmed" inline mt={4}>
              Max 5MB, single image
            </Text>
          </div>
        </Group>
      </Dropzone>
    </AspectRatio>
  );
}
