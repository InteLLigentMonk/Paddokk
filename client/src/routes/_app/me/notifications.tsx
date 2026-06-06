import { createFileRoute } from "@tanstack/react-router";
import { Button, Container, Group, Stack, Text, Title } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { CheckCheck } from "lucide-react";
import {
  notificationsInfiniteQueryOptions,
  unreadCountQueryOptions,
  useMarkAllRead,
} from "@/lib/api/notifications.queries";
import { NotificationList } from "@/components/notifications/notification-list";

export const Route = createFileRoute("/_app/me/notifications")({
  staticData: { breadcrumb: "Notifications" },
  loader: async ({ context: { queryClient } }) => {
    await queryClient.ensureInfiniteQueryData(notificationsInfiniteQueryOptions());
  },
  component: NotificationsPage,
});

function NotificationsPage() {
  const { data: unreadCount = 0 } = useQuery(unreadCountQueryOptions());
  const markAll = useMarkAllRead();

  return (
    <Container size="sm" py="xl">
      <Stack gap="md">
        <Group justify="space-between" align="flex-end" wrap="nowrap">
          <Stack gap={2}>
            <Title order={1}>Notifications</Title>
            <Text c="dimmed">Likes, comments, replies, and new followers</Text>
          </Stack>
          <Button
            variant="subtle"
            size="sm"
            leftSection={<CheckCheck size={16} />}
            onClick={() => markAll.mutate()}
            loading={markAll.isPending}
            disabled={unreadCount === 0}
          >
            Mark all as read
          </Button>
        </Group>

        <NotificationList />
      </Stack>
    </Container>
  );
}
