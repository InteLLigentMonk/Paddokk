import {
  Box,
  Divider,
  Group,
  Image,
  Modal,
  Stack,
  Text,
} from "@mantine/core";
import { JourneyPostComments } from "./journey-post-comments";
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
        <OwnerLink
          target={{ kind: "user", username: post.userUsername }}
          avatarUrl={post.userAvatarUrl}
          primaryText={post.userDisplayName}
          secondaryText={formatDate(post.createdAt)}
          avatarSize="sm"
          avatarRadius="xl"
          primaryTextSize="xs"
        />
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
