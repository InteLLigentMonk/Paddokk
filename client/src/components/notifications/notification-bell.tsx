import { useState } from "react";
import {
  ActionIcon,
  Box,
  Center,
  Divider,
  Group,
  Indicator,
  Loader,
  Menu,
  ScrollArea,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { Link } from "@tanstack/react-router";
import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import { Bell } from "lucide-react";
import { NotificationItem } from "./notification-item";
import type { NotificationDto } from "@/generated/api/schemas";
import {
  notificationsInfiniteQueryOptions,
  unreadCountQueryOptions,
} from "@/lib/api/notifications.queries";
import { useNotificationActivate } from "@/lib/notifications/use-notification-activate";

const DROPDOWN_LIMIT = 8;

export function NotificationBell() {
  const [opened, setOpened] = useState(false);
  const activate = useNotificationActivate();

  const { data: unreadCount = 0 } = useQuery(unreadCountQueryOptions());

  // Recent items load only once the dropdown is opened — opening never marks anything read
  // (anti-dark-pattern, story 12); read state changes only on an explicit row click.
  const { data, isLoading } = useInfiniteQuery({
    ...notificationsInfiniteQueryOptions(),
    enabled: opened,
  });

  const recent = (data?.pages[0]?.items ?? []).slice(0, DROPDOWN_LIMIT);
  const badge = unreadCount > 99 ? "99+" : unreadCount;

  const handleActivate = (notification: NotificationDto) => {
    activate(notification);
    setOpened(false);
  };

  return (
    <Menu
      opened={opened}
      onChange={setOpened}
      position="bottom-end"
      offset={8}
      width={360}
      withArrow
      withinPortal
    >
      <Menu.Target>
        <Indicator
          label={badge}
          size={16}
          color="red"
          offset={4}
          disabled={unreadCount === 0}
          aria-label={`${unreadCount} unread notifications`}
        >
          <ActionIcon variant="default" size="lg" aria-label="Notifications">
            <Bell size={18} strokeWidth={1.5} />
          </ActionIcon>
        </Indicator>
      </Menu.Target>

      <Menu.Dropdown>
        <Group justify="space-between" px="sm" py="xs" wrap="nowrap">
          <Title order={2} fz="md">
            Notifications
          </Title>
          <Text
            component={Link}
            to="/me/notifications"
            size="xs"
            c="blue"
            onClick={() => setOpened(false)}
          >
            See all
          </Text>
        </Group>

        <Divider />

        <NotificationDropdownBody
          isLoading={isLoading}
          items={recent}
          onActivate={handleActivate}
        />
      </Menu.Dropdown>
    </Menu>
  );
}

interface NotificationDropdownBodyProps {
  isLoading: boolean;
  items: Array<NotificationDto>;
  onActivate: (notification: NotificationDto) => void;
}

function NotificationDropdownBody({
  isLoading,
  items,
  onActivate,
}: NotificationDropdownBodyProps) {
  if (isLoading) {
    return (
      <Center py="xl" aria-busy="true">
        <Loader size="sm" />
      </Center>
    );
  }

  if (items.length === 0) {
    return (
      <Center py="xl">
        <Text size="sm" c="dimmed">
          No notifications yet
        </Text>
      </Center>
    );
  }

  return (
    <ScrollArea.Autosize mah={400}>
      <Stack gap={2} p={4}>
        {items.map((notification) => (
          <Box key={notification.id}>
            <NotificationItem
              notification={notification}
              onActivate={onActivate}
            />
          </Box>
        ))}
      </Stack>
    </ScrollArea.Autosize>
  );
}
