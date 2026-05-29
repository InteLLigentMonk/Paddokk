import { useInfiniteQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import {
  Avatar,
  Button,
  Center,
  Group,
  Loader,
  Paper,
  Stack,
  Text,
} from "@mantine/core";
import type { UserDto } from "@/generated/api/schemas";
import type { FollowListType } from "@/lib/api/users.queries";
import { followListInfiniteQueryOptions } from "@/lib/api/users.queries";
import { FollowButton } from "@/components/profile/follow-button";

interface FollowListProps {
  userId: string;
  type: FollowListType;
}

export function FollowList({ userId, type }: FollowListProps) {
  const {
    data,
    isLoading,
    isError,
    hasNextPage,
    fetchNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery(followListInfiniteQueryOptions(userId, type));

  if (isLoading) {
    return (
      <Center py="xl" aria-busy="true">
        <Loader />
      </Center>
    );
  }

  if (isError) {
    return (
      <Text c="dimmed" ta="center" py="xl">
        Could not load {type}. Please try again.
      </Text>
    );
  }

  const users = data?.pages.flatMap((page) => page.items) ?? [];

  if (users.length === 0) {
    return (
      <Text c="dimmed" ta="center" py="xl">
        {type === "followers"
          ? "No followers yet."
          : "Not following anyone yet."}
      </Text>
    );
  }

  return (
    <Stack gap="sm">
      {users.map((user) => (
        <FollowRow key={user.id} user={user} />
      ))}

      {hasNextPage && (
        <Center>
          <Button
            variant="subtle"
            onClick={() => fetchNextPage()}
            loading={isFetchingNextPage}
          >
            Load more
          </Button>
        </Center>
      )}
    </Stack>
  );
}

function FollowRow({ user }: { user: UserDto }) {
  return (
    <Paper withBorder radius="md" p="sm">
      <Group justify="space-between" wrap="nowrap">
        <Link
          to="/users/$username"
          params={{ username: user.username }}
          style={{
            textDecoration: "none",
            color: "inherit",
            minWidth: 0,
            flex: 1,
          }}
        >
          <Group gap="sm" wrap="nowrap">
            <Avatar src={user.avatarUrl ?? undefined} radius="xl">
              {user.displayName.charAt(0) || user.username.charAt(0)}
            </Avatar>
            <Stack gap={0} style={{ minWidth: 0 }}>
              <Text fw={600} truncate>
                {user.displayName}
              </Text>
              <Text size="sm" c="dimmed" truncate>
                @{user.username}
              </Text>
            </Stack>
          </Group>
        </Link>

        <FollowButton
          userId={user.id}
          username={user.username}
          isFollowedByMe={user.isFollowedByMe}
        />
      </Group>
    </Paper>
  );
}
