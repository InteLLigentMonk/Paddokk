import { ActionIcon, Indicator, Menu } from "@mantine/core";
import { Plus, TrendingUp } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import { navigationConfig } from "@/data/navigation/app-navigation";
import { openAddCarModal } from "@/lib/stores/cars-page-store";
import { openCreateJourneyModal } from "@/lib/stores/journeys-page-store";
import { useCanAddJourney } from "@/hooks/use-can-add-journey";

const iconProps = { size: 18, strokeWidth: 1.5 } as const;
const fabIconProps = { size: 24, strokeWidth: 2 } as const;

interface FABMenuProps {
  onActionClick?: (actionId: string) => void;
}

export function FABMenu({ onActionClick }: FABMenuProps) {
  const navigate = useNavigate();
  const { canAdd, currentCount, maxJourneys } = useCanAddJourney();

  const remaining =
    maxJourneys != null && currentCount != null
      ? Number(maxJourneys) - currentCount
      : null;

  const handleAction = (actionId: string) => {
    if (actionId === "add-car") {
      openAddCarModal();
    } else if (actionId === "new-journey") {
      if (canAdd) {
        openCreateJourneyModal();
      } else {
        navigate({ to: "/subscription", search: {} });
      }
    }
    onActionClick?.(actionId);
  };

  return (
    <Menu position="top" withArrow shadow="md" offset={12}>
      <Menu.Target>
        <ActionIcon
          variant="filled"
          size={56}
          radius="xl"
          aria-label="Quick actions"
          style={{
            boxShadow: "var(--mantine-shadow-lg)",
            transform: "translateY(-8px)",
            transition: "all 250ms ease",
          }}
        >
          <Plus {...fabIconProps} />
        </ActionIcon>
      </Menu.Target>

      <Menu.Dropdown>
        <Menu.Label>Quick Actions</Menu.Label>
        {navigationConfig.fabActions.map((action) => {
          if (action.id === "new-journey") {
            const Icon = canAdd ? action.icon : TrendingUp;
            const label = canAdd ? action.label : "Upgrade plan";
            const iconNode = <Icon {...iconProps} />;
            return (
              <Menu.Item
                key={action.id}
                leftSection={iconNode}
                onClick={() => handleAction(action.id)}
              >
                {label}
              </Menu.Item>
            );
          }

          const Icon = action.icon;
          return (
            <Menu.Item
              key={action.id}
              leftSection={<Icon {...iconProps} />}
              onClick={() => handleAction(action.id)}
            >
              {action.label}
            </Menu.Item>
          );
        })}
      </Menu.Dropdown>
    </Menu>
  );
}
