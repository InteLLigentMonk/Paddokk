import { Drawer, Stack, Title, Group, Text, UnstyledButton } from '@mantine/core'
import { Link } from '@tanstack/react-router'
import { navigationConfig } from '@/data/navigation/app-navigation'
import type { NavItem } from '@/data/navigation/types'
import { useCurrentUser } from '@/hooks/use-current-user'

const iconProps = { size: 18, strokeWidth: 1.5 } as const

interface MobileMoreDrawerProps {
  opened: boolean
  onClose: () => void
}

function DrawerNavItem({ item, onClose }: { item: NavItem; onClose: () => void }) {
  const Icon = item.icon
  const { data: me } = useCurrentUser()

  const resolvedHref =
    typeof item.href === 'function'
      ? me?.username
        ? item.href({ username: me.username })
        : undefined
      : item.href

  if (typeof item.href === 'function' && !resolvedHref) return null

  return (
    <UnstyledButton
      component={Link}
      to={resolvedHref!}
      onClick={onClose}
      style={{
        padding: '12px 16px',
        borderRadius: 'var(--mantine-radius-md)',
        transition: 'background-color 150ms ease',
      }}
      styles={{
        root: {
          '&:hover': {
            backgroundColor: 'var(--mantine-color-default-hover)',
          },
        },
      }}
    >
      <Group gap="md">
        <Icon {...iconProps} />
        <Title order={5} fw={500} style={{ flex: 1 }}>
          {item.label}
        </Title>
      </Group>
    </UnstyledButton>
  )
}

export function MobileMoreDrawer({ opened, onClose }: MobileMoreDrawerProps) {
  // Discover: skip first 2 (already in bottom bar: Explore, Journeys)
  const discoverItems = navigationConfig.discover.slice(2)
  const toolItems = navigationConfig.tools

  return (
    <Drawer
      opened={opened}
      onClose={onClose}
      position="left"
      size={280}
      title={
        <Title order={4} fw={600}>
          More
        </Title>
      }
      overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
    >
      <Stack gap="xs">
        <Text size="xs" c="dimmed" px="md" pt="xs" tt="uppercase" fw={600} lts={0.5}>
          Discover
        </Text>
        {discoverItems.map((item) => (
          <DrawerNavItem key={item.id} item={item} onClose={onClose} />
        ))}

        <Text size="xs" c="dimmed" px="md" pt="sm" tt="uppercase" fw={600} lts={0.5}>
          Tools
        </Text>
        {toolItems.map((item) => (
          <DrawerNavItem key={item.id} item={item} onClose={onClose} />
        ))}
      </Stack>
    </Drawer>
  )
}