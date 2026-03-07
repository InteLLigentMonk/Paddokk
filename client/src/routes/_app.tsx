import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { AppHeader } from "@/components/common/app-header";
import { AppSpotlight } from "@/components/common/app-spotlight";
import { AppNavigation } from "@/components/navigation/app-navigation";
import { AddCarModal } from "@/components/cars/add-car-modal";

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
  const isDesktop = useMediaQuery("(min-width: 62em)");

  return (
    <Box>
      <AppSpotlight />
      <AppHeader />
      <AppNavigation />
      <Box
        component="main"
        style={{
          paddingLeft: isDesktop ? 72 : 0,
          paddingBottom: isDesktop ? 0 : 64,
          minHeight: "calc(100vh - 65px)",
        }}
      >
        <Outlet />
      </Box>
      <AddCarModal />
    </Box>
  );
}
