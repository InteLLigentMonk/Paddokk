import { useState } from "react";
import {
  ActionIcon,
  Badge,
  Box,
  Button,
  Group,
  Image,
  Loader,
  Modal,
  SimpleGrid,
  Stack,
  Text,
  TextInput,
} from "@mantine/core";
import { Dropzone, IMAGE_MIME_TYPE } from "@mantine/dropzone";
import { Image as ImageIcon, Upload, X } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import type { JourneyDto } from "@/generated/api/schemas";
import { useNotifications } from "@/integrations/mantine";
import { RichTextEditor } from "@/components/shared/rich-text-editor";
import { useCreateJourneyPost } from "@/hooks/use-journey-detail";
import { getImageLimitsFn } from "@/lib/api/limits";
import {
  journeysDeleteJourneyPostImage,
  journeysUploadJourneyPostImage,
} from "@/generated/api/journeys/journeys";

const SUBSCRIPTION_TIER_LABELS: Record<number, string> = {
  0: "Free",
  1: "Silver",
  2: "Gold",
  3: "Platinum",
  4: "Diamond",
};

interface UploadedImage {
  tempId: string;
  imageUrl: string;
  caption: string;
  sortOrder: number;
  previewUrl: string;
  isUploading: boolean;
}

interface JourneyCreatePostModalProps {
  journey: JourneyDto;
  opened: boolean;
  onClose: () => void;
}

export function JourneyCreatePostModal({
  journey,
  opened,
  onClose,
}: JourneyCreatePostModalProps) {
  const notifications = useNotifications();
  const [content, setContent] = useState("");
  const [editorKey, setEditorKey] = useState(0);
  const [images, setImages] = useState<Array<UploadedImage>>([]);

  const journeyId = Number(journey.id);

  const { data: imageLimits } = useQuery({
    queryKey: ["image-limits"],
    queryFn: () => getImageLimitsFn(),
    enabled: opened,
  });

  const maxImages = Number(imageLimits?.maxImagesPerPost ?? 1);
  const uploadedCount = images.filter((img) => !img.isUploading).length;
  const canAddMoreImages = images.length < maxImages;
  const tierLabel =
    SUBSCRIPTION_TIER_LABELS[Number(imageLimits?.subscriptionTier ?? 0)] ??
    "Free";

  const { mutate: createPost, isPending } = useCreateJourneyPost(journeyId);

  const resetForm = () => {
    setContent("");
    setEditorKey((k) => k + 1);
    setImages([]);
  };

  const cleanupImages = async (toClean: Array<UploadedImage>) => {
    const uploaded = toClean.filter((img) => img.imageUrl && !img.isUploading);
    await Promise.all(
      uploaded.map((img) =>
        journeysDeleteJourneyPostImage(journeyId, {
          journeyId,
          imageUrl: img.imageUrl,
        }).catch(() => {}),
      ),
    );
    toClean.forEach((img) => URL.revokeObjectURL(img.previewUrl));
  };

  const handleClose = async () => {
    await cleanupImages(images);
    resetForm();
    onClose();
  };

  const handleDrop = (files: Array<File>) => {
    const remaining = maxImages - images.length;
    const toUpload = files.slice(0, remaining);

    for (const file of toUpload) {
      const tempId = crypto.randomUUID();
      const previewUrl = URL.createObjectURL(file);

      setImages((prev) => [
        ...prev,
        {
          tempId,
          imageUrl: "",
          caption: "",
          sortOrder: prev.length,
          previewUrl,
          isUploading: true,
        },
      ]);

      journeysUploadJourneyPostImage(journeyId, { file })
        .then((result) => {
          if (result.status === 200) {
            setImages((prev) =>
              prev.map((img) =>
                img.tempId === tempId
                  ? { ...img, imageUrl: result.data.imageUrl, isUploading: false }
                  : img,
              ),
            );
          } else {
            setImages((prev) => prev.filter((img) => img.tempId !== tempId));
            URL.revokeObjectURL(previewUrl);
            notifications.error({ message: "Failed to upload image" });
          }
        })
        .catch(() => {
          setImages((prev) => prev.filter((img) => img.tempId !== tempId));
          URL.revokeObjectURL(previewUrl);
          notifications.error({ message: "Failed to upload image" });
        });
    }
  };

  const handleRemoveImage = (tempId: string) => {
    const image = images.find((img) => img.tempId === tempId);
    setImages((prev) =>
      prev
        .filter((img) => img.tempId !== tempId)
        .map((img, index) => ({ ...img, sortOrder: index })),
    );
    if (image) {
      URL.revokeObjectURL(image.previewUrl);
      if (image.imageUrl) {
        journeysDeleteJourneyPostImage(journeyId, {
          journeyId,
          imageUrl: image.imageUrl,
        }).catch(() => {});
      }
    }
  };

  const handleCaptionChange = (tempId: string, caption: string) => {
    setImages((prev) =>
      prev.map((img) => (img.tempId === tempId ? { ...img, caption } : img)),
    );
  };

  const handleSubmit = () => {
    const uploadedImages = images
      .filter((img) => img.imageUrl && !img.isUploading)
      .map((img) => ({
        imageUrl: img.imageUrl,
        caption: img.caption || null,
        sortOrder: img.sortOrder,
      }));

    const textContent = content.replace(/<[^>]*>/g, "").trim()
      ? content
      : null;

    createPost(
      { textContent, images: uploadedImages },
      {
        onSuccess: () => {
          images.forEach((img) => URL.revokeObjectURL(img.previewUrl));
          resetForm();
          onClose();
        },
        onError: () => {
          notifications.error({ message: "Failed to create post. Please try again." });
        },
      },
    );
  };

  const hasText = content.replace(/<[^>]*>/g, "").trim().length > 0;
  const hasImages = images.some((img) => img.imageUrl && !img.isUploading);
  const isUploading = images.some((img) => img.isUploading);
  const isSubmitDisabled = (!hasText && !hasImages) || isPending || isUploading;

  return (
    <Modal
      opened={opened}
      onClose={handleClose}
      title="New post"
      size="lg"
      closeOnClickOutside={!isPending}
      closeOnEscape={!isPending}
    >
      <Stack gap="md">
        <RichTextEditor
          key={editorKey}
          content={content}
          onChange={setContent}
        />

        {images.length > 0 && (
          <SimpleGrid cols={{ base: 2, sm: 3 }} spacing="sm">
            {images.map((image) => (
              <Box key={image.tempId} style={{ position: "relative" }}>
                <Image
                  src={image.previewUrl}
                  h={120}
                  radius="sm"
                  fit="cover"
                  alt="Post image"
                />
                {image.isUploading && (
                  <Box
                    style={{
                      position: "absolute",
                      inset: 0,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      backgroundColor: "rgba(0,0,0,0.4)",
                      borderRadius: "var(--mantine-radius-sm)",
                    }}
                  >
                    <Loader size="sm" color="white" />
                  </Box>
                )}
                {!image.isUploading && (
                  <ActionIcon
                    size="xs"
                    radius="xl"
                    onClick={() => handleRemoveImage(image.tempId)}
                    style={{ position: "absolute", top: 4, right: 4 }}
                    aria-label="Remove image"
                  >
                    <X size={10} />
                  </ActionIcon>
                )}
                <TextInput
                  placeholder="Caption..."
                  size="xs"
                  mt={4}
                  value={image.caption}
                  onChange={(e) =>
                    handleCaptionChange(image.tempId, e.currentTarget.value)
                  }
                  disabled={image.isUploading}
                  maxLength={500}
                />
              </Box>
            ))}
          </SimpleGrid>
        )}

        {canAddMoreImages && (
          <Dropzone
            onDrop={handleDrop}
            onReject={() =>
              notifications.error({
                message: "Invalid file. Max 10MB, images only.",
              })
            }
            maxSize={10 * 1024 * 1024}
            accept={IMAGE_MIME_TYPE}
            disabled={isPending}
          >
            <Group
              justify="center"
              gap="sm"
              py="sm"
              style={{ pointerEvents: "none" }}
            >
              <Box component="span" darkHidden>
                <Dropzone.Accept>
                  <Upload size={24} style={{ color: "var(--mantine-color-blue-6)" }} />
                </Dropzone.Accept>
                <Dropzone.Reject>
                  <X size={24} style={{ color: "var(--mantine-color-red-6)" }} />
                </Dropzone.Reject>
                <Dropzone.Idle>
                  <ImageIcon size={24} style={{ color: "var(--mantine-color-dimmed)" }} />
                </Dropzone.Idle>
              </Box>
              <Box component="span" lightHidden>
                <Dropzone.Accept>
                  <Upload size={24} style={{ color: "var(--mantine-color-blue-4)" }} />
                </Dropzone.Accept>
                <Dropzone.Reject>
                  <X size={24} style={{ color: "var(--mantine-color-red-4)" }} />
                </Dropzone.Reject>
                <Dropzone.Idle>
                  <ImageIcon size={24} style={{ color: "var(--mantine-color-dimmed)" }} />
                </Dropzone.Idle>
              </Box>
              <Text size="sm" c="dimmed">
                Add photos
              </Text>
            </Group>
          </Dropzone>
        )}

        <Group justify="space-between" align="center">
          <Group gap="xs">
            <Badge variant="light" size="sm">
              {uploadedCount} / {maxImages} images
            </Badge>
            <Badge variant="outline" size="sm" color="gray">
              {tierLabel}
            </Badge>
          </Group>

          <Group gap="sm">
            <Button variant="default" onClick={handleClose} disabled={isPending}>
              Cancel
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={isSubmitDisabled}
              loading={isPending}
            >
              Publish
            </Button>
          </Group>
        </Group>
      </Stack>
    </Modal>
  );
}
