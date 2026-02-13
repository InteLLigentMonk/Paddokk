import { Link } from "@tanstack/react-router"
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
} from "@mantine/core"
import { useDisclosure } from "@mantine/hooks"
import type { NavLink } from "@/data/marketing"
import { mainNavLinks } from "@/data/marketing"
import { openLogin, openSignup } from "@/lib/stores/auth-modal-store"
import { ColorSchemeToggle } from "@/components/common/color-scheme-toggle"

interface NavAnchorProps {
  link: NavLink
  onClick?: () => void
  mobile?: boolean
}

function NavAnchor({ link, onClick, mobile = false }: NavAnchorProps) {
  const commonProps = mobile
    ? {
        w: "100%" as const,
        p: "md",
        style: {
          display: "block",
          borderRadius: "var(--mantine-radius-md)",
          fontSize: "1rem",
          fontWeight: 500,
          color: "var(--mantine-color-text)",
          transition: "all 0.2s cubic-bezier(0.4, 0, 0.2, 1)",
        },
      }
    : {
        fz: "sm" as const,
        fw: 500,
      }

  if (link.type === "anchor") {
    return (
      <UnstyledButton
        component="a"
        href={link.href}
        onClick={onClick}
        {...commonProps}
      >
        {link.label}
      </UnstyledButton>
    )
  }

  return (
    <UnstyledButton
      component={Link}
      to={link.href}
      onClick={onClick}
      {...commonProps}
    >
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
        position: "sticky",
        top: 0,
        zIndex: 100,
        borderBottom: "1px solid var(--mantine-color-default-border)",
        backgroundColor: "var(--mantine-color-body)",
      }}
    >
      <Container size="lg" py="sm">
        <Group justify="space-between">
          {/* Logo / Brand */}
          <Link to="/" style={{ textDecoration: "none" }}>
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
            <ColorSchemeToggle />
            <Button variant="subtle" onClick={openLogin} size="sm">
              Sign In
            </Button>
            <Button onClick={openSignup} size="sm">
              Get Started
            </Button>
          </Group>

          {/* Mobile Burger */}
          <Group gap="xs" hiddenFrom="sm">
            <ColorSchemeToggle />
            <Burger
              opened={drawerOpened}
              onClick={toggleDrawer}
              aria-label="Toggle navigation"
            />
          </Group>
        </Group>
      </Container>

      {/* Mobile Drawer */}
      <Drawer
        opened={drawerOpened}
        onClose={closeDrawer}
        position="right"
        size="80%"
        padding={0}
        withCloseButton={false}
        hiddenFrom="sm"
        zIndex={200}
        styles={{
          content: {
            backgroundColor: "var(--mantine-color-body)",
            backdropFilter: "blur(12px)",
            boxShadow: "-4px 0 24px rgba(0, 0, 0, 0.12)",
          },
          overlay: {
            backdropFilter: "blur(2px)",
            backgroundColor: "rgba(0, 0, 0, 0.25)",
          },
        }}
        transitionProps={{
          transition: "slide-left",
          duration: 250,
          timingFunction: "cubic-bezier(0.4, 0, 0.2, 1)",
        }}
      >
        <Stack gap={0} h="100%">
          {/* Header */}
          <Group
            justify="space-between"
            p="lg"
            style={{
              borderBottom: "1px solid var(--mantine-color-default-border)",
            }}
          >
            <Text fw={700} fz="xl" c="myColor">
              Paddokk
            </Text>
            <Burger
              opened={drawerOpened}
              onClick={closeDrawer}
              aria-label="Close navigation"
              size="sm"
            />
          </Group>

          {/* Navigation Links */}
          <Stack gap={0} p="md" style={{ flex: 1 }}>
            {mainNavLinks.map((link, index) => (
              <Box
                key={link.href}
                style={{
                  opacity: drawerOpened ? 1 : 0,
                  transform: drawerOpened
                    ? "translateX(0)"
                    : "translateX(20px)",
                  transition: `opacity 0.3s ease ${index * 0.05}s, transform 0.3s cubic-bezier(0.4, 0, 0.2, 1) ${index * 0.05}s`,
                }}
              >
                <NavAnchor link={link} onClick={closeDrawer} mobile />
              </Box>
            ))}
          </Stack>

          {/* CTA Buttons */}
          <Stack
            gap="sm"
            p="lg"
            style={{
              borderTop: "1px solid var(--mantine-color-default-border)",
              backgroundColor:
                "light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))",
            }}
          >
            <Button
              variant="light"
              onClick={() => {
                closeDrawer()
                openLogin()
              }}
              fullWidth
              size="md"
              radius="md"
            >
              Sign In
            </Button>
            <Button
              onClick={() => {
                closeDrawer()
                openSignup()
              }}
              fullWidth
              size="md"
              radius="md"
            >
              Get Started
            </Button>
          </Stack>
        </Stack>
      </Drawer>
    </Box>
  )
}
