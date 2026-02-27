import { Link, useMatchRoute } from '@tanstack/react-router'
import { ActionIcon, Tooltip, Indicator } from '@mantine/core'
import type { NavItem as NavItemType } from '@/data/navigation/types'

interface NavItemProps {
  item: NavItemType
  showTooltip?: boolean
  onClick?: () => void
}

const iconProps = { size: 18, strokeWidth: 1.5 } as const

export function NavItem({ item, showTooltip = false, onClick }: NavItemProps) {
  const matchRoute = useMatchRoute()
  const isActive = item.href ? !!matchRoute({ to: item.href }) : false

  const icon = <item.icon {...iconProps} />

  const button = (
    <Indicator
      disabled={!item.badge || item.badge === 0}
      label={item.badge}
      size={16}
      offset={4}
    >
      <ActionIcon
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component={item.href ? (Link as any) : 'button'}
        to={item.href}
        variant={isActive ? 'filled' : 'subtle'}
        color={isActive ? undefined : 'dark'}
        size="xl"
        aria-label={item.label}
        aria-current={isActive ? 'page' : undefined}
        onClick={onClick}
        style={{
          transition: 'all 250ms ease',
        }}
      >
        {icon}
      </ActionIcon>
    </Indicator>
  )

  if (showTooltip && item.label) {
    return (
      <Tooltip label={item.label} position="right" withArrow openDelay={150}>
        {button}
      </Tooltip>
    )
  }

  return button
}
