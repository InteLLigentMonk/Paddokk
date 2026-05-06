import { Modal, Image, Text, Stack } from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import type { JourneyPostImageDto } from "@/generated/api/schemas";

interface PostImageModalProps {
  images: JourneyPostImageDto[];
  initialIndex: number;
  onClose: () => void;
}

export function PostImageModal({
  images,
  initialIndex,
  onClose,
}: PostImageModalProps) {
  return (
    <Modal
      opened
      onClose={onClose}
      size="100%"
      centered
      padding={16}
      styles={{
        content: { background: "#000" },
        header: {
          position: "absolute",
          top: 0,
          right: 0,
          zIndex: 10,
          background: "transparent",
        },
        body: { padding: 0 },
        close: {
          color: "var(--mantine-color-white)",
          background: "rgba(255,255,255,0.15)",
        },
      }}
      overlayProps={{ backgroundOpacity: 0.9 }}
    >
      <Carousel
        initialSlide={initialIndex}
        withControls={images.length > 1}
        height="auto"
        styles={{
          control: {
            background: "rgba(255,255,255,0.15)",
            border: "none",
            color: "white",
          },
          indicator: { background: "rgba(255,255,255,0.6)" },
        }}
      >
        {images.map((img) => (
          <Carousel.Slide key={String(img.id)}>
            <Stack gap={0}>
              <Image
                src={img.imageUrl}
                alt={img.caption ?? ""}
                fit="contain"
                mah="75vh"
                w="100%"
              />
              {img.caption && (
                <Text
                  size="sm"
                  c="white"
                  ta="center"
                  px="md"
                  py="xs"
                  opacity={0.7}
                >
                  {img.caption}
                </Text>
              )}
            </Stack>
          </Carousel.Slide>
        ))}
      </Carousel>
    </Modal>
  );
}
