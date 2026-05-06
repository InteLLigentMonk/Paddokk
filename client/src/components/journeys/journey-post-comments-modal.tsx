import {
  Modal,
  Group,
  Avatar,
  Stack,
  Text,
  Image,
  Divider,
  Box,
} from "@mantine/core";
import type { JourneyPostDto } from "@/generated/api/schemas";
import { JourneyPostComments } from "./journey-post-comments";

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("sv-SE", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

interface PostPreviewProps {
  post: JourneyPostDto;
}

function PostPreview({ post }: PostPreviewProps) {
  const firstImage =
    post.images.length > 0
      ? [...post.images].sort(
          (a, b) => Number(a.sortOrder) - Number(b.sortOrder),
        )[0]
      : null;

  return (
    <Box mb="md">
      <Group gap="sm" wrap="nowrap" mb="xs">
        <Avatar
          src={post.userAvatarUrl ?? null}
          radius="xl"
          size="sm"
          alt={post.userDisplayName}
        />
        <Stack gap={0}>
          <Text size="xs" fw={600}>
            {post.userDisplayName}
          </Text>
          <Text size="xs" c="dimmed">
            {formatDate(post.createdAt)}
          </Text>
        </Stack>
        {firstImage && (
          <Image
            src={firstImage.imageUrl}
            alt={firstImage.caption ?? ""}
            w={48}
            h={48}
            radius="sm"
            fit="cover"
            ml="auto"
          />
        )}
      </Group>
      {post.textContent && (
        <div
          dangerouslySetInnerHTML={{ __html: post.textContent }}
          style={{
            fontSize: "var(--mantine-font-size-sm)",
            color: "var(--mantine-color-dimmed)",
            display: "-webkit-box",
            WebkitLineClamp: 2,
            WebkitBoxOrient: "vertical",
            overflow: "hidden",
            wordBreak: "break-word",
          }}
        />
      )}
      <Divider mt="md" />
    </Box>
  );
}

interface JourneyPostCommentsModalProps {
  post: JourneyPostDto | null;
  onClose: () => void;
}

export function JourneyPostCommentsModal({
  post,
  onClose,
}: JourneyPostCommentsModalProps) {
  return (
    <Modal
      opened={post !== null}
      onClose={onClose}
      title={<Text fw={600}>Kommentarer</Text>}
      size="lg"
      styles={{
        content: {
          display: "flex",
          flexDirection: "column",
          height: "90dvh",
        },
        header: { flexShrink: 0 },
        body: {
          flex: 1,
          minHeight: 0,
          display: "flex",
          flexDirection: "column",
          overflow: "hidden",
          padding: 0,
        },
      }}
    >
      {post !== null && (
        <>
          <Box px="md" pt="xs" style={{ flexShrink: 0 }}>
            <PostPreview post={post} />
          </Box>
          <Stack px="md" pb="md" flex={1} mih={0} gap={0}>
            <JourneyPostComments
              postId={Number(post.id)}
              isPostOwner={post.isOwner}
              stretch
            />
          </Stack>
        </>
      )}
    </Modal>
  );
}
