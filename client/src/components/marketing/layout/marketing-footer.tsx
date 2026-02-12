import { Box, Container, Divider, SimpleGrid, Stack, Text } from '@mantine/core'
import { footerNavLinks, legalLinks } from '@/data/marketing'

export function MarketingFooter() {
  const currentYear = new Date().getFullYear()

  return (
    <Box component="footer" py="xl" style={{ borderTop: '1px solid var(--mantine-color-gray-3)' }}>
      <Container size="lg">
        <SimpleGrid cols={{ base: 1, sm: 3 }} spacing="xl">
          {/* Brand Column */}
          <Stack gap="xs">
            <Text fw={700} fz="lg" c="myColor">
              Paddokk
            </Text>
            <Text fz="sm" c="dimmed">
              The social platform for car enthusiasts.
              Document your journey, share your builds.
            </Text>
          </Stack>

          {/* Navigation Column */}
          <Stack gap="xs">
            <Text fw={600} fz="sm">
              Navigation
            </Text>
            {footerNavLinks.map((link) => (
              <Text
                key={link.href}
                component="a"
                href={link.href}
                fz="sm"
                c="dimmed"
                style={{ textDecoration: 'none' }}
              >
                {link.label}
              </Text>
            ))}
          </Stack>

          {/* Legal Column */}
          <Stack gap="xs">
            <Text fw={600} fz="sm">
              Legal
            </Text>
            {legalLinks.map((link) => (
              <Text
                key={link.href}
                component="a"
                href={link.href}
                fz="sm"
                c="dimmed"
                style={{ textDecoration: 'none' }}
              >
                {link.label}
              </Text>
            ))}
          </Stack>
        </SimpleGrid>

        <Divider my="xl" />

        <Text fz="xs" c="dimmed" ta="center">
          {currentYear} Paddokk. All rights reserved.
        </Text>
      </Container>
    </Box>
  )
}
