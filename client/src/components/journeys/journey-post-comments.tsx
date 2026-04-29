import { useState } from "react";
import {
  Stack,
  Group,
  Avatar,
  Text,
  Textarea,
  Button,
  ScrollArea,
  Divider,
  ActionIcon,
} from "@mantine/core";
import { Send, Trash2 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import {
  postCommentsQueryOptions,
  useCreateComment,
} from "@/hooks/use-journey-detail";
import { ExpandableText } from "@/components/common/expandable-text";
import type { PostCommentDto } from "@/generated/api/schemas";

function formatRelativeDate(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diff / 60_000);
  if (minutes < 1) return "Just nu";
  if (minutes < 60) return `${minutes} min sedan`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours} tim sedan`;
  const days = Math.floor(hours / 24);
  if (days < 7) return `${days} dagar sedan`;
  return new Date(iso).toLocaleDateString("sv-SE", {
    day: "numeric",
    month: "short",
  });
}

interface CommentItemProps {
  comment: PostCommentDto;
}

function CommentItem({ comment }: CommentItemProps) {
  return (
    <Group align="flex-start" gap="xs" wrap="nowrap">
      <Avatar
        src={comment.userAvatarUrl ?? null}
        size="sm"
        radius="xl"
        alt={comment.userDisplayName}
      />
      <Stack gap={2} flex={1} miw={0}>
        <Group gap="xs" wrap="nowrap">
          <Text size="xs" fw={600} style={{ whiteSpace: "nowrap" }}>
            {comment.userDisplayName}
          </Text>
          <Text size="xs" c="dimmed">
            {formatRelativeDate(comment.createdAt)}
          </Text>
          {comment.isEdited && (
            <Text size="xs" c="dimmed" fs="italic">
              (redigerad)
            </Text>
          )}
        </Group>
        <ExpandableText
          text={comment.content}
          maxLines={2}
          charsPerLine={50}
          size="sm"
        />
      </Stack>
      {comment.isOwner && (
        <ActionIcon
          variant="subtle"
          size="sm"
          color="red"
          aria-label="Ta bort kommentar"
        >
          <Trash2 size={14} />
        </ActionIcon>
      )}
    </Group>
  );
}

interface JourneyPostCommentsProps {
  postId: number;
  maxHeight?: number | string;
  stretch?: boolean;
}

export function JourneyPostComments({
  postId,
  maxHeight = 360,
  stretch = false,
}: JourneyPostCommentsProps) {
  const [text, setText] = useState("");
  const { data } = useQuery(postCommentsQueryOptions(postId));
  const { mutate: createComment, isPending } = useCreateComment(postId);

  const comments = data?.comments ?? [];

  const handleSubmit = () => {
    const trimmed = text.trim();
    if (!trimmed) return;
    createComment(trimmed, { onSuccess: () => setText("") });
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
      e.preventDefault();
      handleSubmit();
    }
  };

  const commentList =
    comments.length === 0 ? (
      <Text size="sm" c="dimmed" ta="center" py="md">
        Inga kommentarer än — var först!
      </Text>
    ) : (
      <Stack gap="sm" pr="xs">
        {comments.map((comment, i) => (
          <div key={String(comment.id)}>
            <CommentItem comment={comment} />
            {i < comments.length - 1 && <Divider mt="xs" />}
          </div>
        ))}
      </Stack>
    );

  const input = (
    <Stack gap="xs">
      <Textarea
        placeholder="Skriv en kommentar... (Ctrl+Enter för att skicka)"
        value={text}
        onChange={(e) => setText(e.currentTarget.value)}
        onKeyDown={handleKeyDown}
        autosize
        minRows={2}
        maxRows={4}
        disabled={isPending}
      />
      <Group justify="flex-end">
        <Button
          size="xs"
          leftSection={<Send size={14} />}
          onClick={handleSubmit}
          disabled={!text.trim()}
          loading={isPending}
        >
          Skicka
        </Button>
      </Group>
    </Stack>
  );

  if (stretch) {
    return (
      <Stack flex={1} mih={0} gap={0}>
        <ScrollArea
          flex={1}
          mih={0}
          offsetScrollbars
          styles={{ viewport: { height: "100%" } }}
        >
          {commentList}
        </ScrollArea>
        <Divider my="sm" />
        {input}
      </Stack>
    );
  }

  return (
    <Stack gap="sm" h="100%">
      <ScrollArea mah={maxHeight} offsetScrollbars>
        {commentList}
      </ScrollArea>
      <Divider />
      {input}
    </Stack>
  );
}
