import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box, Container, Text } from "@mantine/core";
import { ColorSchemeToggle } from "@/components/common/color-scheme-toggle";

export const Route = createFileRoute("/_auth")({
  beforeLoad: ({ context }) => {
    // Redirect authenticated users to the app
    if (context.auth.isAuthenticated) {
      throw redirect({ to: "/dashboard" });
    }
  },
  component: AuthLayout,
});

function AuthLayout() {
  return (
    <Box
      style={{
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        padding: "var(--mantine-spacing-md)",
        position: "relative",
      }}
    >
      {/* Color scheme toggle */}
      <Box style={{ position: "absolute", top: 16, right: 16 }}>
        <ColorSchemeToggle />
      </Box>

      <Container size="xs" w="100%">
        <Outlet />
      </Container>

      <Text
        size="xs"
        c="dimmed"
        ta="center"
        mt="xl"
        style={{ position: "absolute", bottom: 20 }}
      >
        © {new Date().getFullYear()} Paddokk. All rights reserved.
      </Text>
    </Box>
  );
}
