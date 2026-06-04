import { Button, Center, Stack, Text } from "@mantine/core";
import { Rss } from "lucide-react";
import { Link } from "@tanstack/react-router";

/**
 * Shown to a logged-in User whose Follow/Subscription graph is empty. Honest empty state
 * with CTAs — never a populated fallback masquerading as their feed (ADR-0004, #161).
 */
export function FeedEmptyState() {
  return (
    <Center py="xl">
      <Stack align="center" gap="md" maw={360}>
        <Rss size={48} strokeWidth={1.2} color="var(--mantine-color-dimmed)" />
        <Stack align="center" gap={4}>
          <Text fw={600}>Your feed is empty</Text>
          <Text size="sm" c="dimmed" ta="center">
            Follow people and subscribe to cars and journeys you care about, and
            their updates will show up here.
          </Text>
        </Stack>
        <Stack gap="xs" w="100%">
          <Button component={Link} to="/community" variant="filled" fullWidth>
            Find people to Follow
          </Button>
          <Button component={Link} to="/journeys" variant="light" fullWidth>
            Browse Journeys
          </Button>
        </Stack>
      </Stack>
    </Center>
  );
}
