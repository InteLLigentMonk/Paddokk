import { createFileRoute, notFound } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { z } from "zod";
import {
  Avatar,
  Container,
  Group,
  Stack,
  Tabs,
  Text,
  Title,
  UnstyledButton,
} from "@mantine/core";
import { userByUsernameQueryOptions } from "@/lib/api/users.queries";
import { FollowButton } from "@/components/profile/follow-button";
import { FollowList } from "@/components/profile/follow-list";

const profileSearchSchema = z.object({
  tab: z.enum(["followers", "following"]).optional(),
});

export const Route = createFileRoute("/_app/users/$username/")({
  validateSearch: profileSearchSchema,
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
  const { tab } = Route.useSearch();
  const navigate = Route.useNavigate();
  // Read from the live query (seeded by the loader) so optimistic follow
  // updates and their rollback are reflected immediately in the UI.
  const { data: user } = useSuspenseQuery(userByUsernameQueryOptions(username));

  const activeTab = tab ?? "followers";
  const selectTab = (value: "followers" | "following") =>
    navigate({ search: { tab: value }, replace: true });

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
          <UnstyledButton
            onClick={() => selectTab("followers")}
            aria-label="Show followers"
          >
            <Stack gap={0} align="center">
              <Text fw={600}>{user.followerCount}</Text>
              <Text size="sm" c="dimmed">
                Followers
              </Text>
            </Stack>
          </UnstyledButton>
          <UnstyledButton
            onClick={() => selectTab("following")}
            aria-label="Show following"
          >
            <Stack gap={0} align="center">
              <Text fw={600}>{user.followingCount}</Text>
              <Text size="sm" c="dimmed">
                Following
              </Text>
            </Stack>
          </UnstyledButton>
        </Group>

        <Tabs
          value={activeTab}
          onChange={(value) =>
            selectTab(value === "following" ? "following" : "followers")
          }
          keepMounted={false}
        >
          <Tabs.List>
            <Tabs.Tab value="followers">Followers</Tabs.Tab>
            <Tabs.Tab value="following">Following</Tabs.Tab>
          </Tabs.List>

          <Tabs.Panel value="followers" pt="lg">
            <FollowList userId={user.id} type="followers" />
          </Tabs.Panel>
          <Tabs.Panel value="following" pt="lg">
            <FollowList userId={user.id} type="following" />
          </Tabs.Panel>
        </Tabs>
      </Stack>
    </Container>
  );
}
