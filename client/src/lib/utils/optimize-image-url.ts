export function optimizeImageUrl(
  src: string | null | undefined,
  width: number,
  quality = 80,
): string | undefined {
  if (!src) return undefined;
  if (import.meta.env.DEV) return src;
  if (!src.startsWith("http")) return src;

  const params = new URLSearchParams({
    url: src,
    w: String(width),
    q: String(quality),
  });
  return `/_vercel/image?${params}`;
}
