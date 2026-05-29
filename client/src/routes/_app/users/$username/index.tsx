import { createFileRoute, notFound } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Avatar, Container, Group, Stack, Text, Title } from "@mantine/core";
import { userByUsernameQueryOptions } from "@/lib/api/users.queries";
import { FollowButton } from "@/components/profile/follow-button";

export const Route = createFileRoute("/_app/users/$username/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await queryClient.ensureQueryData(
        userByUsernameQueryOptions(params.username),
      );
    } catch {
      throw notFound();
    }
  },
  component: UserProfilePage,
});

function UserProfilePage() {
  const { username } = Route.useParams();
  // Read from the live query (seeded by the loader) so optimistic follow
  // updates and their rollback are reflected immediately in the UI.
  const { data: user } = useSuspenseQuery(userByUsernameQueryOptions(username));

  const fullName = [user.firstName, user.lastName]
    .filter(Boolean)
    .join(" ")
    .trim();

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <Group gap="lg" align="center" justify="space-between">
          <Group gap="lg" align="center">
            <Avatar src={user.avatarUrl ?? undefined} size={96} radius="xl">
              {user.firstName.charAt(0) || user.username.charAt(0)}
            </Avatar>
            <Stack gap={4}>
              <Title order={2}>{user.displayName}</Title>
              <Text c="dimmed">@{user.username}</Text>
              {fullName && fullName !== user.displayName && (
                <Text size="sm" c="dimmed">
                  {fullName}
                </Text>
              )}
            </Stack>
          </Group>
          <FollowButton
            userId={user.id}
            username={user.username}
            isFollowedByMe={user.isFollowedByMe}
          />
        </Group>

        {user.bio && <Text>{user.bio}</Text>}

        <Group gap="lg">
          <Stack gap={0} align="center">
            <Text fw={600}>{user.carCount}</Text>
            <Text size="sm" c="dimmed">
              Cars
            </Text>
          </Stack>
          <Stack gap={0} align="center">
            <Text fw={600}>{user.journeyCount}</Text>
            <Text size="sm" c="dimmed">
              Journeys
            </Text>
          </Stack>
          <Stack gap={0} align="center">
            <Text fw={600}>{user.followerCount}</Text>
            <Text size="sm" c="dimmed">
              Followers
            </Text>
          </Stack>
          <Stack gap={0} align="center">
            <Text fw={600}>{user.followingCount}</Text>
            <Text size="sm" c="dimmed">
              Following
            </Text>
          </Stack>
        </Group>
      </Stack>
    </Container>
  );
}
