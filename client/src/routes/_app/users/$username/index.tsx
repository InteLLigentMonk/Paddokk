import { createFileRoute, notFound } from "@tanstack/react-router";
import { Container, Stack, Title, Text, Avatar, Group } from "@mantine/core";
import { getUserByUsernameFn } from "@/lib/api/users.server";

export const Route = createFileRoute("/_app/users/$username/")({
  loader: async ({ params }) => {
    try {
      return await getUserByUsernameFn({ data: { username: params.username } });
    } catch {
      throw notFound();
    }
  },
  component: UserProfilePage,
});

function UserProfilePage() {
  const user = Route.useLoaderData();

  const fullName = [user.firstName, user.lastName]
    .filter(Boolean)
    .join(" ")
    .trim();

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <Group gap="lg" align="center">
          <Avatar src={user.avatarUrl ?? undefined} size={96} radius="xl">
            {user.firstName?.[0] ?? user.username[0]}
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
        </Group>
      </Stack>
    </Container>
  );
}
