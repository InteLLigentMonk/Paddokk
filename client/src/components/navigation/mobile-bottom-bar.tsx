import { Box, Group } from "@mantine/core";
import { Menu } from "lucide-react";
import { NavItem } from "./nav-item";
import { FABMenu } from "./fab-menu";
import type { NavItem as NavItemType } from "@/data/navigation/types";
import { navigationConfig } from "@/data/navigation/app-navigation";

const MORE_ITEM: NavItemType = {
  id: "more",
  label: "More",
  icon: Menu,
  group: "me",
};

interface MobileBottomBarProps {
  onMoreClick: () => void;
}

export function MobileBottomBar({ onMoreClick }: MobileBottomBarProps) {
  // Me: exclude desktopOnly items → [Feed, My Journeys, My Cars]
  const leftItems = navigationConfig.me.filter((item) => !item.desktopOnly);

  // Discover: first 2 items → [Explore, Journeys], then More
  const rightItems = [...navigationConfig.discover.slice(0, 2), MORE_ITEM];

  return (
    <Box
      component="nav"
      aria-label="Main navigation"
      style={{
        position: "fixed",
        bottom: 0,
        left: 0,
        right: 0,
        height: 64,
        borderTop: "1px solid var(--mantine-color-default-border)",
        backgroundColor: "var(--mantine-color-body)",
        backdropFilter: "blur(12px)",
        zIndex: 100,
        paddingBottom: "env(safe-area-inset-bottom)",
      }}
    >
      <Group
        justify="space-around"
        align="center"
        h="100%"
        px="xs"
        gap={0}
        wrap="nowrap"
        style={{ position: "relative" }}
      >
        {leftItems.map((item) => (
          <NavItem key={item.id} item={item} />
        ))}

        <Box style={{ flexShrink: 0 }}>
          <FABMenu />
        </Box>

        {rightItems.map((item) => {
          if (item.id === "more") {
            return <NavItem key={item.id} item={item} onClick={onMoreClick} />;
          }
          return <NavItem key={item.id} item={item} />;
        })}
      </Group>
    </Box>
  );
}
