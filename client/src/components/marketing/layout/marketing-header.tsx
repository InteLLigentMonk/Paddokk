import { Link } from "@tanstack/react-router";
import {
  Box,
  Burger,
  Button,
  Container,
  Divider,
  Drawer,
  Group,
  Stack,
  Text,
  UnstyledButton,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { NavLink } from "@/data/marketing";
import { mainNavLinks } from "@/data/marketing";

function NavAnchor({ link, onClick }: { link: NavLink; onClick?: () => void }) {
  if (link.type === "anchor") {
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
    );
  }

  return (
    <UnstyledButton
      component={Link}
      to={link.href}
      onClick={onClick}
      fz="sm"
      fw={500}
    >
      {link.label}
    </UnstyledButton>
  );
}

export function MarketingHeader() {
  const [drawerOpened, { toggle: toggleDrawer, close: closeDrawer }] =
    useDisclosure(false);

  return (
    <Box
      component="header"
      style={{
        position: "sticky",
        top: 0,
        zIndex: 100,
        borderBottom: "1px solid var(--mantine-color-gray-3)",
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
        position="right"
        size="80%"
        padding="md"
        title="Paddokk"
        hiddenFrom="sm"
        zIndex={200}
      >
        <Stack gap={0}>
          {/* Navigation Links */}
          <Stack gap="xs">
            {mainNavLinks.map((link) => (
              <UnstyledButton
                key={link.href}
                component={link.type === "anchor" ? "a" : Link}
                href={link.type === "anchor" ? link.href : undefined}
                to={link.type === "route" ? link.href : undefined}
                onClick={closeDrawer}
                p="sm"
                style={(theme) => ({
                  borderRadius: theme.radius.sm,
                  transition: "background-color 0.15s ease",
                  ":hover": {
                    backgroundColor: theme.colors.gray[0],
                  },
                })}
              >
                <Text fw={500} fz="md">
                  {link.label}
                </Text>
              </UnstyledButton>
            ))}
          </Stack>

          <Divider my="md" />

          {/* CTA Buttons */}
          <Stack gap="sm">
            <Button
              variant="subtle"
              component={Link}
              to="/login"
              onClick={closeDrawer}
              fullWidth
            >
              Sign In
            </Button>
            <Button
              component={Link}
              to="/signup"
              onClick={closeDrawer}
              fullWidth
            >
              Get Started
            </Button>
          </Stack>
        </Stack>
      </Drawer>
    </Box>
  );
}
