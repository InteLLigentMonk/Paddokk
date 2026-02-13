import { createFileRoute } from "@tanstack/react-router";
import { Card, Container, SimpleGrid, Stack, Text, Title } from "@mantine/core";
import { BookOpen, Camera, Car, Users } from "lucide-react";

export const Route = createFileRoute("/_app/dashboard")({
  component: AppDashboard,
});

function AppDashboard() {
  const { auth } = Route.useRouteContext();

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        {/* Welcome Header */}
        <Stack gap="xs">
          <Title order={1}>Welcome back, {auth.user?.name}!</Title>
          <Text c="dimmed">
            Ready to document your latest journey or connect with fellow
            enthusiasts?
          </Text>
        </Stack>

        {/* Quick Actions */}
        <SimpleGrid cols={{ base: 1, sm: 2, md: 4 }} spacing="lg">
          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Stack gap="md" align="center">
              <Car
                size={32}
                style={{ color: "var(--mantine-primary-color-filled)" }}
              />
              <Stack gap="xs" align="center">
                <Text fw={600}>My Journeys</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Track your builds and trips
                </Text>
              </Stack>
            </Stack>
          </Card>

          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Stack gap="md" align="center">
              <Camera
                size={32}
                style={{ color: "var(--mantine-primary-color-filled)" }}
              />
              <Stack gap="xs" align="center">
                <Text fw={600}>Photo Gallery</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Share your best shots
                </Text>
              </Stack>
            </Stack>
          </Card>

          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Stack gap="md" align="center">
              <Users
                size={32}
                style={{ color: "var(--mantine-primary-color-filled)" }}
              />
              <Stack gap="xs" align="center">
                <Text fw={600}>Community</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Connect with enthusiasts
                </Text>
              </Stack>
            </Stack>
          </Card>

          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Stack gap="md" align="center">
              <BookOpen
                size={32}
                style={{ color: "var(--mantine-primary-color-filled)" }}
              />
              <Stack gap="xs" align="center">
                <Text fw={600}>Knowledge Base</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Learn from the community
                </Text>
              </Stack>
            </Stack>
          </Card>
        </SimpleGrid>

        {/* Placeholder Content */}
        <Card shadow="sm" padding="xl" radius="md" withBorder>
          <Stack gap="md">
            <Title order={3}>Your Feed</Title>
            <Text c="dimmed">
              This is a placeholder for your personalized feed. Features coming
              soon!
            </Text>
          </Stack>
        </Card>
      </Stack>
    </Container>
  );
}
