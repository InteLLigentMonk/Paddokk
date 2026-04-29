import { useState } from "react";
import {
  Card,
  Grid,
  Group,
  Stack,
  Text,
  Avatar,
  Button,
  Anchor,
  Image,
  Box,
  Paper,
  Divider,
} from "@mantine/core";
import { Carousel } from "@mantine/carousel";
import { MessageSquare, ChevronDown, ChevronUp } from "lucide-react";
import type { JourneyPostDto } from "@/generated/api/schemas";
import { JourneyPostComments } from "./journey-post-comments";
import { JourneyPostCommentsModal } from "./journey-post-comments-modal";
import { PostImageModal } from "./journey-post-image-modal";

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
    <Group gap="sm" wrap="nowrap">
      <Avatar
        src={post.userAvatarUrl ?? null}
        radius="xl"
        size="md"
        alt={post.userDisplayName}
      />
      <Stack gap={0}>
        <Text fw={600} size="sm">
          {post.userDisplayName}
        </Text>
        <Text size="xs" c="dimmed">
          {formatDate(post.createdAt)}
          {post.isEdited && " · redigerad"}
        </Text>
      </Stack>
    </Group>
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
      <Image
        src={sorted[0].imageUrl}
        alt={sorted[0].caption ?? ""}
        radius="sm"
        fit="cover"
        mah={320}
        onClick={() => onImageClick(0)}
        style={imgStyle}
      />
    );
  }

  return (
    <Carousel withIndicators height="100%" slideSize="90%" slideGap="xs">
      {sorted.map((img, i) => (
        <Carousel.Slide key={String(img.id)}>
          <Image
            src={img.imageUrl}
            alt={img.caption ?? ""}
            fit="cover"
            h="100%"
            radius="sm"
            onClick={() => onImageClick(i)}
            style={imgStyle}
          />
        </Carousel.Slide>
      ))}
    </Carousel>
  );
}

interface PostTextProps {
  text: string;
}

function PostText({ text }: PostTextProps) {
  const [expanded, setExpanded] = useState(false);
  const lines = text.split("\n");
  const showToggle = lines.length > 5;
  const preview = lines.slice(0, 5).join("\n");

  return (
    <Stack gap="xs">
      <Text style={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
        {expanded ? text : preview}
        {!expanded && showToggle && "…"}
      </Text>
      {showToggle && (
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
  const postId = Number(post.id);

  const sortedImages = [...post.images].sort(
    (a, b) => Number(a.sortOrder) - Number(b.sortOrder),
  );

  const postContent = (
    <Stack gap="md">
      <PostHeader post={post} />
      <PostImages images={post.images} onImageClick={setLightboxIndex} />
      {post.textContent && <PostText text={post.textContent} />}
      <Divider hiddenFrom="md" />
      <Group hiddenFrom="md">
        <Button
          variant="subtle"
          size="sm"
          leftSection={<MessageSquare size={14} />}
          onClick={() => setCommentModalOpen(true)}
        >
          {Number(post.commentCount) > 0
            ? `${Number(post.commentCount)} kommentarer`
            : "Kommentera"}
        </Button>
      </Group>
    </Stack>
  );

  return (
    <>
      {/* Mobile layout */}
      <Card withBorder radius="md" padding="md" hiddenFrom="md">
        {postContent}
      </Card>

      {/* Desktop layout */}
      <Paper
        withBorder
        radius="md"
        p="md"
        visibleFrom="md"
        bg="light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))"
      >
        <Grid gutter="md">
          <Grid.Col span={7}>{postContent}</Grid.Col>
          <Grid.Col span={5}>
            <Box
              pl="md"
              h="100%"
              style={{
                borderLeft: "1px solid var(--mantine-color-default-border)",
              }}
            >
              <Text size="xs" fw={600} c="dimmed" tt="uppercase" mb="sm">
                Kommentarer
              </Text>
              <JourneyPostComments postId={postId} />
            </Box>
          </Grid.Col>
        </Grid>
      </Paper>

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
