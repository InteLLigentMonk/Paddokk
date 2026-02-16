import { Box, Group } from '@mantine/core'
import { NavItem } from './nav-item'
import { FABMenu } from './fab-menu'
import { navigationConfig } from '@/data/navigation/app-navigation'

interface MobileBottomBarProps {
  onMoreClick: () => void
}

export function MobileBottomBar({ onMoreClick }: MobileBottomBarProps) {
  // Get first 3 primary items (Dashboard, Journeys, Explore)
  const leftItems = navigationConfig.primary.slice(0, 3)

  // Get last 3 primary items (Cars, Community, More)
  const rightItems = navigationConfig.primary.slice(3)

  return (
    <Box
      component="nav"
      aria-label="Main navigation"
      style={{
        position: 'fixed',
        bottom: 0,
        left: 0,
        right: 0,
        height: 64,
        borderTop: '1px solid var(--mantine-color-default-border)',
        backgroundColor: 'var(--mantine-color-body)',
        backdropFilter: 'blur(12px)',
        zIndex: 100,
        paddingBottom: 'env(safe-area-inset-bottom)',
      }}
    >
      <Group
        justify="space-around"
        align="center"
        h="100%"
        px="xs"
        gap={0}
        wrap="nowrap"
        style={{ position: 'relative' }}
      >
        {/* Left section - 3 items */}
        {leftItems.map((item) => (
          <NavItem key={item.id} item={item} />
        ))}

        {/* Center FAB */}
        <Box style={{ flexShrink: 0 }}>
          <FABMenu />
        </Box>

        {/* Right section - 3 items */}
        {rightItems.map((item) => {
          // Special handling for "More" button
          if (item.id === 'more') {
            return (
              <NavItem
                key={item.id}
                item={item}
                onClick={onMoreClick}
              />
            )
          }
          return <NavItem key={item.id} item={item} />
        })}
      </Group>
    </Box>
  )
}
