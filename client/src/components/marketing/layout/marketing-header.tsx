import { Link } from '@tanstack/react-router'
import {
  Box,
  Burger,
  Button,
  Container,
  Drawer,
  Group,
  Stack,
  Text,
  UnstyledButton,
} from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { mainNavLinks } from '@/data/marketing'
import type { NavLink } from '@/data/marketing'

function NavAnchor({ link, onClick }: { link: NavLink; onClick?: () => void }) {
  if (link.type === 'anchor') {
    return (
      <UnstyledButton
        component="a"
        href={link.href}
        onClick={onClick}
        fz="sm"
        fw={500}
      >
        {link.label}
      </UnstyledButton>
    )
  }

  return (
    <UnstyledButton component={Link} to={link.href} onClick={onClick} fz="sm" fw={500}>
      {link.label}
    </UnstyledButton>
  )
}

export function MarketingHeader() {
  const [drawerOpened, { toggle: toggleDrawer, close: closeDrawer }] =
    useDisclosure(false)

  return (
    <Box
      component="header"
      style={{
        position: 'sticky',
        top: 0,
        zIndex: 100,
        borderBottom: '1px solid var(--mantine-color-gray-3)',
        backgroundColor: 'var(--mantine-color-body)',
      }}
    >
      <Container size="lg" py="sm">
        <Group justify="space-between">
          {/* Logo / Brand */}
          <Link to="/" style={{ textDecoration: 'none' }}>
            <Text fw={700} fz="xl" c="myColor">
              Paddokk
            </Text>
          </Link>

          {/* Desktop Nav */}
          <Group gap="lg" visibleFrom="sm">
            {mainNavLinks.map((link) => (
              <NavAnchor key={link.href} link={link} />
            ))}
          </Group>

          {/* Desktop CTA */}
          <Group gap="sm" visibleFrom="sm">
            <Button variant="subtle" component={Link} to="/login" size="sm">
              Sign In
            </Button>
            <Button component={Link} to="/signup" size="sm">
              Get Started
            </Button>
          </Group>

          {/* Mobile Burger */}
          <Burger
            opened={drawerOpened}
            onClick={toggleDrawer}
            hiddenFrom="sm"
            aria-label="Toggle navigation"
          />
        </Group>
      </Container>

      {/* Mobile Drawer */}
      <Drawer
        opened={drawerOpened}
        onClose={closeDrawer}
        size="100%"
        padding="md"
        title="Paddokk"
        hiddenFrom="sm"
        zIndex={200}
      >
        <Stack gap="lg">
          {mainNavLinks.map((link) => (
            <NavAnchor key={link.href} link={link} onClick={closeDrawer} />
          ))}
          <Button variant="subtle" component={Link} to="/login" onClick={closeDrawer}>
            Sign In
          </Button>
          <Button component={Link} to="/signup" onClick={closeDrawer}>
            Get Started
          </Button>
        </Stack>
      </Drawer>
    </Box>
  )
}
