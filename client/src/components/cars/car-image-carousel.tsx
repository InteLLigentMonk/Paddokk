import { AspectRatio } from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import type { CarImageDto } from "@/generated/api/schemas";
import { CdnImage } from "@/components/shared/cdn-image";

interface CarImageCarouselProps {
  images: Array<CarImageDto>;
  displayName: string;
}

export function CarImageCarousel({
  images,
  displayName,
}: CarImageCarouselProps) {
  if (images.length === 0) {
    return (
      <AspectRatio ratio={16 / 9}>
        <CdnImage
          src={null}
          placeholder="car"
          alt={displayName}
          radius="md"
        />
      </AspectRatio>
    );
  }

  if (images.length === 1) {
    return (
      <AspectRatio ratio={16 / 9}>
        <CdnImage
          src={images[0].imageUrl}
          width={1600}
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
            <CdnImage
              src={img.imageUrl}
              width={1600}
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
