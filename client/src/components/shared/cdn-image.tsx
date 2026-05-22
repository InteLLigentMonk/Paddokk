import { Image } from "@mantine/core";
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
  loading?: ImgHTMLAttributes<HTMLImageElement>["loading"];
  decoding?: ImgHTMLAttributes<HTMLImageElement>["decoding"];
  draggable?: boolean;
  onClick?: MouseEventHandler<HTMLImageElement>;
  onDragStart?: DragEventHandler<HTMLImageElement>;
}

export function CdnImage({
  src,
  width = 800,
  quality = 80,
  ...rest
}: CdnImageProps) {
  return <Image src={optimizeImageUrl(src, width, quality)} {...rest} />;
}
