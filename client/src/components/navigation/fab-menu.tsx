import { ActionIcon, Menu } from '@mantine/core'
import { Plus } from 'lucide-react'
import { navigationConfig } from '@/data/navigation/app-navigation'

const iconProps = { size: 18, strokeWidth: 1.5 } as const
const fabIconProps = { size: 24, strokeWidth: 2 } as const

interface FABMenuProps {
  onActionClick?: (actionId: string) => void
}

export function FABMenu({ onActionClick }: FABMenuProps) {
  const handleAction = (actionId: string) => {
    // TODO: Implement actual actions based on actionId
    console.log('FAB action:', actionId)
    onActionClick?.(actionId)
  }

  return (
    <Menu position="top" withArrow shadow="md" offset={12}>
      <Menu.Target>
        <ActionIcon
          variant="filled"
          size={56}
          radius="xl"
          aria-label="Quick actions"
          style={{
            boxShadow: 'var(--mantine-shadow-lg)',
            transform: 'translateY(-8px)',
            transition: 'all 250ms ease',
          }}
        >
          <Plus {...fabIconProps} />
        </ActionIcon>
      </Menu.Target>

      <Menu.Dropdown>
        <Menu.Label>Quick Actions</Menu.Label>
        {navigationConfig.fabActions.map((action) => {
          const Icon = action.icon
          return (
            <Menu.Item
              key={action.id}
              leftSection={<Icon {...iconProps} />}
              onClick={() => handleAction(action.id)}
            >
              {action.label}
            </Menu.Item>
          )
        })}
      </Menu.Dropdown>
    </Menu>
  )
}
