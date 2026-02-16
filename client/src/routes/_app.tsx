import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box } from "@mantine/core";
import { AppHeader } from "@/components/common/app-header";
import { AppSpotlight } from "@/components/common/app-spotlight";

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
  return (
    <Box>
      <AppSpotlight />
      <AppHeader />
      <Box component="main">
        <Outlet />
      </Box>
    </Box>
  );
}
