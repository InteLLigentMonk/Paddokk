import { Link, useMatchRoute } from "@tanstack/react-router";
import { ActionIcon, Tooltip, Indicator } from "@mantine/core";
import type { NavItem as NavItemType } from "@/data/navigation/types";
import { useCurrentUser } from "@/hooks/use-current-user";

interface NavItemProps {
  item: NavItemType;
  showTooltip?: boolean;
  onClick?: () => void;
}

const iconProps = { size: 18, strokeWidth: 1.5 } as const;

export function NavItem({ item, showTooltip = false, onClick }: NavItemProps) {
  const matchRoute = useMatchRoute();
  const { data: me } = useCurrentUser();

  const resolvedHref =
    typeof item.href === "function"
      ? me?.username
        ? item.href({ username: me.username })
        : undefined
      : item.href;

  // Hide items that depend on a username until it has resolved
  if (typeof item.href === "function" && !resolvedHref) return null;

  const isActive = resolvedHref
    ? !!matchRoute({ to: resolvedHref, fuzzy: true })
    : false;

  const icon = <item.icon {...iconProps} />;

  const button = (
    <Indicator
      disabled={!item.badge || item.badge === 0}
      label={item.badge}
      size={16}
      offset={4}
    >
      <ActionIcon
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component={resolvedHref ? (Link as any) : "button"}
        to={resolvedHref}
        variant={isActive ? "filled" : "subtle"}
        color={isActive ? undefined : "dark"}
        size="xl"
        aria-label={item.label}
        aria-current={isActive ? "page" : undefined}
        onClick={onClick}
        style={{
          transition: "all 250ms ease",
        }}
      >
        {icon}
      </ActionIcon>
    </Indicator>
  );

  if (showTooltip && item.label) {
    return (
      <Tooltip label={item.label} position="right" withArrow openDelay={150}>
        {button}
      </Tooltip>
    );
  }

  return button;
}
