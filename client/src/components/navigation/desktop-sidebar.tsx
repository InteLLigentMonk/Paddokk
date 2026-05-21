import { ActionIcon, Box, Divider, Stack, Tooltip } from '@mantine/core'
import { LogOut } from 'lucide-react'
import { NavItem } from './nav-item'
import { navigationConfig } from '@/data/navigation/app-navigation'
import { useAuth } from '@/hooks/use-auth'

const iconProps = { size: 18, strokeWidth: 1.5 } as const

export function DesktopSidebar() {
  const { logout, isLoggingOut } = useAuth()

  const meItems = navigationConfig.me.filter((item) => !item.mobileOnly)
  const discoverItems = navigationConfig.discover
  const toolItems = navigationConfig.tools

  return (
    <Box
      component="nav"
      aria-label="Main navigation"
      style={{
        position: 'fixed',
        left: 0,
        top: 65,
        width: 72,
        height: 'calc(100vh - 65px)',
        borderRight: '1px solid var(--mantine-color-default-border)',
        backgroundColor: 'var(--mantine-color-body)',
        zIndex: 50,
        display: 'flex',
        flexDirection: 'column',
      }}
    >
      <Stack gap="sm" p="md" align="center" style={{ flex: 1 }}>
        {meItems.map((item) => (
          <NavItem key={item.id} item={item} showTooltip />
        ))}

        <Divider style={{ width: '100%' }} my="xs" />

        {discoverItems.map((item) => (
          <NavItem key={item.id} item={item} showTooltip />
        ))}

        <Divider style={{ width: '100%' }} my="xs" />

        {toolItems.map((item) => (
          <NavItem key={item.id} item={item} showTooltip />
        ))}
      </Stack>

      <Box p="md" style={{ borderTop: '1px solid var(--mantine-color-default-border)' }}>
        <Tooltip label="Sign out" position="right" withArrow openDelay={150}>
          <ActionIcon
            variant="subtle"
            color="red"
            size="xl"
            aria-label="Sign out"
            onClick={logout}
            disabled={isLoggingOut}
            style={{
              transition: 'all 250ms ease',
            }}
          >
            <LogOut {...iconProps} />
          </ActionIcon>
        </Tooltip>
      </Box>
    </Box>
  )
}
