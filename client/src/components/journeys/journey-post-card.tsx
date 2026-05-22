import { useEffect, useRef, useState } from "react";
import {
  Anchor,
  AspectRatio,
  Button,
  Card,
  Group,
  Stack,
} from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import { ChevronDown, ChevronUp, MessageSquare } from "lucide-react";
import { JourneyPostCommentsModal } from "./journey-post-comments-modal";
import { PostImageModal } from "./journey-post-image-modal";
import { CdnImage } from "@/components/shared/cdn-image";
import type { JourneyPostDto } from "@/generated/api/schemas";
import { OwnerLink } from "@/components/common/owner-link";

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("sv-SE", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

interface PostHeaderProps {
  post: JourneyPostDto;
}

function PostHeader({ post }: PostHeaderProps) {
  return (
    <OwnerLink
      target={{ kind: "user", username: post.userUsername }}
      avatarUrl={post.userAvatarUrl}
      primaryText={post.userDisplayName}
      secondaryText={
        <>
          {formatDate(post.createdAt)}
          {post.isEdited && " · redigerad"}
        </>
      }
      avatarSize="md"
      avatarRadius="xl"
    />
  );
}

interface PostImagesProps {
  images: JourneyPostDto["images"];
  onImageClick: (index: number) => void;
}

function PostImages({ images, onImageClick }: PostImagesProps) {
  if (images.length === 0) return null;

  const sorted = [...images].sort(
    (a, b) => Number(a.sortOrder) - Number(b.sortOrder),
  );

  const imgStyle = { cursor: "pointer" };

  if (sorted.length === 1) {
    return (
      <AspectRatio ratio={16 / 9}>
        <CdnImage
          src={sorted[0].imageUrl}
          width={1200}
          alt={sorted[0].caption ?? ""}
          radius="sm"
          fit="cover"
          onClick={() => onImageClick(0)}
          style={imgStyle}
        />
      </AspectRatio>
    );
  }

  return (
    <AspectRatio ratio={16 / 9}>
      <Carousel height="100%" slideSize="100%" slideGap="xs">
        {sorted.map((img, i) => (
          <Carousel.Slide key={String(img.id)}>
            <CdnImage
              src={img.imageUrl}
              width={1200}
              alt={img.caption ?? ""}
              fit="cover"
              h="100%"
              onClick={() => onImageClick(i)}
              style={imgStyle}
            />
          </Carousel.Slide>
        ))}
      </Carousel>
    </AspectRatio>
  );
}

interface PostTextProps {
  text: string;
}

function PostText({ text }: PostTextProps) {
  const [expanded, setExpanded] = useState(false);
  const [overflows, setOverflows] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (ref.current) {
      setOverflows(ref.current.scrollHeight > ref.current.clientHeight + 1);
    }
  }, [text]);

  return (
    <Stack gap="xs">
      <div
        ref={ref}
        dangerouslySetInnerHTML={{ __html: text }}
        style={{
          wordBreak: "break-word",
          ...(!expanded && {
            display: "-webkit-box",
            WebkitLineClamp: 8,
            WebkitBoxOrient: "vertical",
            overflow: "hidden",
          }),
        }}
      />
      {(overflows || expanded) && (
        <Anchor
          component="button"
          size="xs"
          onClick={() => setExpanded((v) => !v)}
        >
          <Group gap={4} align="center" wrap="nowrap">
            {expanded ? <ChevronUp size={14} /> : <ChevronDown size={14} />}
            {expanded ? "Visa mindre" : "Visa mer"}
          </Group>
        </Anchor>
      )}
    </Stack>
  );
}

interface JourneyPostCardProps {
  post: JourneyPostDto;
}

export function JourneyPostCard({ post }: JourneyPostCardProps) {
  const [commentModalOpen, setCommentModalOpen] = useState(false);
  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);

  const sortedImages = [...post.images].sort(
    (a, b) => Number(a.sortOrder) - Number(b.sortOrder),
  );

  return (
    <>
      <Card
        withBorder
        radius="md"
        padding="sm"
        bg="light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))"
      >
        <Card.Section withBorder inheritPadding py="sm">
          <PostHeader post={post} />
        </Card.Section>
        {post.images.length > 0 && (
          <Card.Section>
            <PostImages images={post.images} onImageClick={setLightboxIndex} />
          </Card.Section>
        )}
        <Card.Section
          withBorder
          inheritPadding
          py="sm"
          bg="light-dark(var(--mantine-color-white), var(--mantine-color-dark-8))"
        >
          {post.textContent && <PostText text={post.textContent} />}
        </Card.Section>
        <Card.Section withBorder inheritPadding py="xs">
          <Button
            variant="subtle"
            size="sm"
            leftSection={<MessageSquare size={14} />}
            onClick={() => setCommentModalOpen(true)}
          >
            {Number(post.commentCount) > 0
              ? `${Number(post.commentCount)}`
              : "Comment"}
          </Button>
        </Card.Section>
      </Card>

      {/* Mobile comments modal */}
      <JourneyPostCommentsModal
        post={commentModalOpen ? post : null}
        onClose={() => setCommentModalOpen(false)}
      />

      {lightboxIndex !== null && (
        <PostImageModal
          images={sortedImages}
          initialIndex={lightboxIndex}
          onClose={() => setLightboxIndex(null)}
        />
      )}
    </>
  );
}
