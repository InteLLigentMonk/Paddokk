import { Anchor, Box, Button, Group, Paper, Stack, Text } from "@mantine/core";
import { Cookie } from "lucide-react";
import { useConsentControls } from "@/lib/consent/consent-context";

/**
 * Bottom-anchored cookie consent banner. Renders only while no decision has
 * been recorded. Essential cookies stay active regardless of the choice; the
 * two actions only differ on non-essential cookies.
 */
export function ConsentBanner() {
  const { hasDecision, acceptAll, rejectNonEssential } = useConsentControls();

  if (hasDecision) return null;

  return (
    <Box
      role="region"
      aria-label="Cookie consent"
      pos="fixed"
      bottom={0}
      left={0}
      right={0}
      p="md"
      style={{ zIndex: 200 }}
    >
      <Paper
        withBorder
        radius="md"
        p="lg"
        shadow="md"
        maw={960}
        mx="auto"
        bg="var(--mantine-color-body)"
      >
        <Stack gap="md">
          <Group gap="sm" wrap="nowrap" align="flex-start">
            <Cookie
              size={20}
              aria-hidden
              style={{ flexShrink: 0, marginTop: 2 }}
            />
            <Text size="sm">
              We use essential cookies to make Paddokk work. With your consent
              we also use non-essential cookies to help us improve the product.
              See our{" "}
              <Anchor href="/privacy" size="sm">
                Privacy Policy
              </Anchor>{" "}
              for details.
            </Text>
          </Group>

          <Group justify="flex-end" gap="sm">
            <Button variant="default" onClick={rejectNonEssential}>
              Reject non-essential
            </Button>
            <Button onClick={acceptAll}>Accept all</Button>
          </Group>
        </Stack>
      </Paper>
    </Box>
  );
}
