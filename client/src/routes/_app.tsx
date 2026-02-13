import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Avatar, Box, Button, Container, Group, Text } from "@mantine/core";
import { useAuth } from "@/hooks/use-auth";
import { ColorSchemeToggle } from "@/components/common/color-scheme-toggle";

export const Route = createFileRoute("/_app")({
  beforeLoad: ({ context }) => {
    // Redirect unauthenticated users to landing page
    if (!context.auth.isAuthenticated) {
      throw redirect({ to: "/" });
    }
  },
  component: AppLayout,
});

function AppLayout() {
  const { user, logout, isLoggingOut } = useAuth();

  return (
    <Box>
      {/* App Header */}
      <Box
        component="header"
        style={{
          borderBottom: "1px solid var(--mantine-color-default-border)",
          backgroundColor: "var(--mantine-color-body)",
        }}
      >
        <Container size="lg" py="sm">
          <Group justify="space-between">
            <Text fw={700} fz="xl" c="myColor">
              Paddokk
            </Text>

            <Group gap="md">
              <ColorSchemeToggle />
              {user && (
                <>
                  <Group gap="xs">
                    <Avatar size="sm" radius="xl">
                      {user.name.charAt(0).toUpperCase()}
                    </Avatar>
                    <Text size="sm" fw={500}>
                      {user.name}
                    </Text>
                  </Group>
                  <Button
                    variant="subtle"
                    size="sm"
                    onClick={() => logout()}
                    loading={isLoggingOut}
                  >
                    Sign out
                  </Button>
                </>
              )}
            </Group>
          </Group>
        </Container>
      </Box>

      {/* App Content */}
      <Box component="main">
        <Outlet />
      </Box>
    </Box>
  );
}
