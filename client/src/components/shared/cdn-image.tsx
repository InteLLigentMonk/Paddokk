import { Image } from "@mantine/core";
import { NoImagePlaceholder } from "./no-image-placeholder";
import type { PlaceholderVariant } from "./no-image-placeholder";
import type { ImageProps } from "@mantine/core";
import type {
  DragEventHandler,
  ImgHTMLAttributes,
  MouseEventHandler,
} from "react";
import { optimizeImageUrl } from "@/lib/utils/optimize-image-url";

interface CdnImageProps extends Omit<ImageProps, "src"> {
  src: string | null | undefined;
  width?: number;
  quality?: number;
  alt?: string;
  placeholder?: PlaceholderVariant;
  loading?: ImgHTMLAttributes<HTMLImageElement>["loading"];
  decoding?: ImgHTMLAttributes<HTMLImageElement>["decoding"];
  draggable?: boolean;
  onClick?: MouseEventHandler<HTMLImageElement>;
  onDragStart?: DragEventHandler<HTMLImageElement>;
}

export function CdnImage({
  src,
  width = 600,
  quality = 80,
  placeholder = "photo",
  ...rest
}: CdnImageProps) {
  if (!src)
    return <NoImagePlaceholder variant={placeholder} label={rest.alt} />;
  return <Image src={optimizeImageUrl(src, width, quality)} {...rest} />;
}
