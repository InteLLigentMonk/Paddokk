import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { Box, ScrollArea } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { AppHeader } from "@/components/common/app-header";
import { AppSpotlight } from "@/components/common/app-spotlight";
import { AppNavigation } from "@/components/navigation/app-navigation";
import { AddCarModal } from "@/components/cars/add-car-modal";
import { CreateJourneyModal } from "@/components/journeys/create-journey-modal";

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
          flex: 1,
          minHeight: 0,
          overflowY: "auto",
          paddingLeft: isDesktop ? 72 : 0,
        }}
      >
        <ScrollArea
          h={{ base: "calc(100dvh - 65px - 64px)", md: "calc(100dvh - 65px)" }}
        >
          <Outlet />
        </ScrollArea>
      </Box>
      <AddCarModal />
      <CreateJourneyModal />
    </Box>
  );
}
