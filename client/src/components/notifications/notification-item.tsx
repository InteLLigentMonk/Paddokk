import { Avatar, Box, Group, Stack, Text, UnstyledButton } from "@mantine/core";
import type { NotificationDto } from "@/generated/api/schemas";
import { notificationMessage } from "@/lib/notifications/notification-display";
import { absoluteTime, relativeTime } from "@/lib/feed/feed-time";

interface NotificationItemProps {
  notification: NotificationDto;
  onActivate: (notification: NotificationDto) => void;
}

/**
 * One notification row, shared by the bell dropdown and the hub. Unread rows are tinted and
 * carry a dot; the whole row is a single button so the entire surface is a 44px+ touch target
 * and reachable by keyboard.
 */
export function NotificationItem({
  notification,
  onActivate,
}: NotificationItemProps) {
  const unread = !notification.read;

  return (
    <UnstyledButton
      onClick={() => onActivate(notification)}
      aria-label={`${notification.actorDisplayName} ${notificationMessage(notification)}`}
      w="100%"
      p="sm"
      style={{
        borderRadius: "var(--mantine-radius-sm)",
        backgroundColor: unread
          ? "var(--mantine-primary-color-light)"
          : "transparent",
      }}
    >
      <Group gap="sm" wrap="nowrap" align="flex-start">
        <Avatar
          src={notification.actorAvatarUrl}
          alt={notification.actorDisplayName}
          radius="xl"
          size="md"
        >
          {notification.actorDisplayName.charAt(0).toUpperCase()}
        </Avatar>

        <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
          <Text size="sm" lineClamp={2}>
            <Text span fw={600}>
              {notification.actorDisplayName}
            </Text>{" "}
            {notificationMessage(notification)}
          </Text>
          <Text size="xs" c="dimmed" title={absoluteTime(notification.createdAt)}>
            {relativeTime(notification.createdAt)}
          </Text>
        </Stack>

        {unread && (
          <Box
            w={8}
            h={8}
            aria-hidden
            style={{
              flexShrink: 0,
              borderRadius: "50%",
              marginTop: "var(--mantine-spacing-xs)",
              backgroundColor: "var(--mantine-primary-color-filled)",
            }}
          />
        )}
      </Group>
    </UnstyledButton>
  );
}
