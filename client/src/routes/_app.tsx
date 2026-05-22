import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box } from "@mantine/core";
import { AppHeader } from "@/components/common/app-header";
import { AppSpotlight } from "@/components/common/app-spotlight";
import { AppNavigation } from "@/components/navigation/app-navigation";
import { AddCarModal } from "@/components/cars/add-car-modal";
import { CreateJourneyModal } from "@/components/journeys/create-journey-modal";
import { PageBreadcrumbs } from "@/components/common/page-breadcrumbs";
import { currentUserQueryOptions } from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app")({
  beforeLoad: async ({ context: { queryClient, auth } }) => {
    // Redirect unauthenticated users to landing page
    if (!auth.isAuthenticated) {
      throw redirect({ to: "/" });
    }

    // Prefetch current user profile (needed for navigation items with dynamic usernames)
    await queryClient.ensureQueryData(currentUserQueryOptions());
  },
  component: AppLayout,
});

function AppLayout() {
  return (
    <Box
      style={{
        display: "flex",
        flexDirection: "column",
        height: "100dvh",
        overflow: "hidden",
      }}
      pb={{ base: "64px", md: 0 }}
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
        }}
        pl={{ base: 0, md: "72px" }}
      >
        <PageBreadcrumbs />
        <Outlet />
      </Box>
      <AddCarModal />
      <CreateJourneyModal />
    </Box>
  );
}
