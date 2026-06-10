import { Link } from "@tanstack/react-router";
import {
  Box,
  Container,
  Divider,
  SimpleGrid,
  Stack,
  Text,
} from "@mantine/core";
import type { NavLink } from "@/data/marketing";
import { footerNavLinks, legalLinks } from "@/data/marketing";
import { ManageCookies } from "@/components/consent";

/** Render a footer link, using SPA navigation for in-app routes. */
function FooterLink({ link }: { link: NavLink }) {
  const commonProps = {
    fz: "sm" as const,
    c: "dimmed" as const,
    style: { textDecoration: "none" },
  };

  if (link.type === "route") {
    return (
      <Text component={Link} to={link.href} {...commonProps}>
        {link.label}
      </Text>
    );
  }

  return (
    <Text component="a" href={link.href} {...commonProps}>
      {link.label}
    </Text>
  );
}

export function MarketingFooter() {
  const currentYear = new Date().getFullYear();

  return (
    <Box
      component="footer"
      py="xl"
      style={{ borderTop: "1px solid var(--mantine-color-default-border)" }}
    >
      <Container size="lg">
        <SimpleGrid cols={{ base: 1, sm: 3 }} spacing="xl">
          {/* Brand Column */}
          <Stack gap="xs">
            <Text fw={700} fz="lg" c="myColor">
              Paddokk
            </Text>
            <Text fz="sm" c="dimmed">
              The social platform for car enthusiasts. Document your journey,
              share your builds.
            </Text>
          </Stack>

          {/* Navigation Column */}
          <Stack gap="xs">
            <Text fw={600} fz="sm">
              Navigation
            </Text>
            {footerNavLinks.map((link) => (
              <FooterLink key={link.href} link={link} />
            ))}
          </Stack>

          {/* Legal Column */}
          <Stack gap="xs">
            <Text fw={600} fz="sm">
              Legal
            </Text>
            {legalLinks.map((link) => (
              <FooterLink key={link.href} link={link} />
            ))}
            <ManageCookies />
          </Stack>
        </SimpleGrid>

        <Divider my="xl" />

        <Text fz="xs" c="dimmed" ta="center">
          {currentYear} Paddokk. All rights reserved.
        </Text>
      </Container>
    </Box>
  );
}
