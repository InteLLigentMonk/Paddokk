import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { AppHeader } from "@/components/common/app-header";
import { AppSpotlight } from "@/components/common/app-spotlight";
import { AppNavigation } from "@/components/navigation/app-navigation";
import { AddCarModal } from "@/components/cars/add-car-modal";
import { CreateJourneyModal } from "@/components/journeys/create-journey-modal";
import { PageBreadcrumbs } from "@/components/common/page-breadcrumbs";

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
    <Box
      style={{
        display: "flex",
        flexDirection: "column",
        height: "100dvh",
        overflow: "hidden",
        paddingBottom: isDesktop ? 0 : 64,
      }}
    >
      <AppSpotlight />
      <AppHeader />
      <AppNavigation />
      <Box
        component="main"
        style={{
          position: "relative",
          flex: 1,
          minHeight: 0,
          overflowY: "auto",
          paddingLeft: isDesktop ? 72 : 0,
        }}
      >
        <PageBreadcrumbs />
        <Outlet />
      </Box>
      <AddCarModal />
      <CreateJourneyModal />
    </Box>
  );
}
