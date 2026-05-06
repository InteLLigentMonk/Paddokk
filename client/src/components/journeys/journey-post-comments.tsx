import { useState } from "react";
import {
  Stack,
  Group,
  Avatar,
  Text,
  Textarea,
  ScrollArea,
  Divider,
  ActionIcon,
  Paper,
  Box,
  Button,
} from "@mantine/core";
import { Send, Trash2, Reply } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import {
  postCommentsQueryOptions,
  useCreateComment,
  useDeleteComment,
  useReplyToComment,
} from "@/hooks/use-journey-detail";
import { ExpandableText } from "@/components/common/expandable-text";
import type { PostCommentDto } from "@/generated/api/schemas";

const MAX_COMMENT_LENGTH = 500;

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

interface InlineTextareaProps {
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  isPending: boolean;
  placeholder?: string;
}

function InlineTextarea({
  value,
  onChange,
  onSubmit,
  isPending,
  placeholder,
}: InlineTextareaProps) {
  const atMax = value.length >= MAX_COMMENT_LENGTH;

  return (
    <Stack gap={2}>
      <Box style={{ position: "relative" }}>
        <Textarea
          placeholder={placeholder ?? "Skriv..."}
          value={value}
          onChange={(e) => onChange(e.currentTarget.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
              e.preventDefault();
              onSubmit();
            }
          }}
          autosize
          minRows={1}
          maxRows={6}
          maxLength={MAX_COMMENT_LENGTH}
          disabled={isPending}
          styles={{ input: { paddingRight: "2.5rem" } }}
        />
        <ActionIcon
          style={{ position: "absolute", bottom: 6, right: 6 }}
          size="sm"
          variant="subtle"
          onClick={onSubmit}
          disabled={!value.trim() || isPending}
          loading={isPending}
          aria-label="Skicka"
        >
          <Send size={14} />
        </ActionIcon>
      </Box>
      <Text size="xs" c={atMax ? "red" : "dimmed"} ta="right">
        {value.length}/{MAX_COMMENT_LENGTH}
      </Text>
    </Stack>
  );
}

interface CommentItemProps {
  comment: PostCommentDto;
  postId: number;
  isPostOwner: boolean;
}

function CommentItem({ comment, postId, isPostOwner }: CommentItemProps) {
  const [showReplyInput, setShowReplyInput] = useState(false);
  const [replyText, setReplyText] = useState("");
  const { mutate: deleteComment, isPending: isDeleting } =
    useDeleteComment(postId);
  const { mutate: reply, isPending: isReplying } = useReplyToComment(postId);

  const handleReply = () => {
    const trimmed = replyText.trim();
    if (!trimmed) return;
    reply(
      { content: trimmed, parentCommentId: Number(comment.id) },
      {
        onSuccess: () => {
          setReplyText("");
          setShowReplyInput(false);
        },
      },
    );
  };

  return (
    <Paper
      p="sm"
      bg="light-dark(var(--mantine-color-gray-1), var(--mantine-color-dark-7))"
      withBorder
      radius="md"
    >
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
                (edited)
              </Text>
            )}
          </Group>
          <ExpandableText text={comment.content} maxLines={2} />
          {isPostOwner && !comment.reply && !showReplyInput && (
            <Button
              variant="subtle"
              size="compact-xs"
              color="gray"
              leftSection={<Reply size={12} />}
              onClick={() => setShowReplyInput((v) => !v)}
              mt={2}
              w="fit-content"
            >
              Reply
            </Button>
          )}
        </Stack>
        {comment.isOwner && (
          <ActionIcon
            variant="subtle"
            size="sm"
            color="red"
            aria-label="Delete comment"
            onClick={() => deleteComment(Number(comment.id))}
            loading={isDeleting}
          >
            <Trash2 size={14} />
          </ActionIcon>
        )}
      </Group>

      {comment.reply && (
        <Group
          bg="light-dark(var(--mantine-color-white), var(--mantine-color-dark-8))"
          p="sm"
          bdrs="md"
          w="fit-content"
          align="flex-start"
          gap="xs"
          wrap="nowrap"
          justify="flex-end"
          mt="xs"
          styles={{ root: { marginLeft: "auto" } }}
        >
          <Stack gap={2} miw={0} maw="90%" style={{ alignItems: "flex-end" }}>
            <Group gap="xs" wrap="nowrap">
              {comment.reply.isOwner && (
                <ActionIcon
                  variant="subtle"
                  size="sm"
                  color="red"
                  aria-label="Delete reply"
                  onClick={() => deleteComment(Number(comment.reply!.id))}
                  loading={isDeleting}
                >
                  <Trash2 size={14} />
                </ActionIcon>
              )}
              <Text size="xs" c="dimmed">
                {formatRelativeDate(comment.reply.createdAt)}
              </Text>
              <Text size="xs" fw={600} style={{ whiteSpace: "nowrap" }}>
                {comment.reply.userDisplayName}
              </Text>
            </Group>
            <Text size="sm" ta="right">
              {comment.reply.content}
            </Text>
          </Stack>
          <Avatar
            src={comment.reply.userAvatarUrl ?? null}
            size="sm"
            radius="xl"
            alt={comment.reply.userDisplayName}
          />
        </Group>
      )}

      {showReplyInput && !comment.reply && (
        <Box mt="xs">
          <InlineTextarea
            value={replyText}
            onChange={setReplyText}
            onSubmit={handleReply}
            isPending={isReplying}
            placeholder="Skriv ett svar..."
          />
        </Box>
      )}
    </Paper>
  );
}

interface JourneyPostCommentsProps {
  postId: number;
  isPostOwner?: boolean;
  maxHeight?: number | string;
  stretch?: boolean;
}

export function JourneyPostComments({
  postId,
  isPostOwner = false,
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

  const commentList =
    comments.length === 0 ? (
      <Text size="sm" c="dimmed" ta="center" py="md">
        Inga kommentarer än — bli den första!
      </Text>
    ) : (
      <Stack gap="sm" pr="xs">
        {comments.map((comment) => (
          <CommentItem
            key={String(comment.id)}
            comment={comment}
            postId={postId}
            isPostOwner={isPostOwner}
          />
        ))}
      </Stack>
    );

  const input = (
    <InlineTextarea
      value={text}
      onChange={setText}
      onSubmit={handleSubmit}
      isPending={isPending}
      placeholder="Skriv en kommentar..."
    />
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
