import { AspectRatio, Image } from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import type { CarImageDto } from "@/generated/api/schemas";

interface CarImageCarouselProps {
  images: Array<CarImageDto>;
  displayName: string;
}

export function CarImageCarousel({ images, displayName }: CarImageCarouselProps) {
  if (images.length === 0) {
    return (
      <AspectRatio ratio={16 / 9}>
        <Image
          src="https://placehold.co/800x450/e9ecef/495057?text=No+Photos"
          alt={displayName}
          radius="md"
          fit="cover"
        />
      </AspectRatio>
    );
  }

  if (images.length === 1) {
    return (
      <AspectRatio ratio={16 / 9}>
        <Image
          src={images[0].imageUrl}
          alt={images[0].caption ?? displayName}
          radius="md"
          fit="cover"
        />
      </AspectRatio>
    );
  }

  return (
    <AspectRatio ratio={16 / 9}>
      <Carousel withIndicators slideGap="md" emblaOptions={{ loop: true }}>
        {images.map((img) => (
          <Carousel.Slide key={String(img.id)}>
            <Image
              src={img.imageUrl}
              alt={img.caption ?? displayName}
              fit="contain"
              bg="black"
              radius="md"
            />
          </Carousel.Slide>
        ))}
      </Carousel>
    </AspectRatio>
  );
}
