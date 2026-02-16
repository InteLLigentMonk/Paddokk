import { Drawer, Stack, Title, Group, UnstyledButton } from '@mantine/core'
import { Link } from '@tanstack/react-router'
import { navigationConfig } from '@/data/navigation/app-navigation'

const iconProps = { size: 18, strokeWidth: 1.5 } as const

interface MobileMoreDrawerProps {
  opened: boolean
  onClose: () => void
}

export function MobileMoreDrawer({ opened, onClose }: MobileMoreDrawerProps) {
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
        {navigationConfig.secondary.map((item) => {
          const Icon = item.icon
          return (
            <UnstyledButton
              key={item.id}
              component={Link}
              to={item.href!}
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
        })}
      </Stack>
    </Drawer>
  )
}
