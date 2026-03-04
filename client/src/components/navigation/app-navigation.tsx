import { useDisclosure } from "@mantine/hooks";
import { Box } from "@mantine/core";
import { DesktopSidebar } from "./desktop-sidebar";
import { MobileBottomBar } from "./mobile-bottom-bar";
import { MobileMoreDrawer } from "./mobile-more-drawer";

export function AppNavigation() {
  const [moreDrawerOpened, { toggle, close }] = useDisclosure();

  return (
    <>
      <Box visibleFrom="md">
        <DesktopSidebar />
      </Box>
      <Box hiddenFrom="md">
        <MobileBottomBar onMoreClick={toggle} />
        <MobileMoreDrawer opened={moreDrawerOpened} onClose={close} />
      </Box>
    </>
  );
}
