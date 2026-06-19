import { Button, Container, Stack, Text, Title } from "@mantine/core";
import { Link } from "@tanstack/react-router";
import { Compass } from "lucide-react";

/**
 * Full-page 404 state shown by the root `notFoundComponent` for unmatched
 * paths. Uses the global layout and a single way back into the app.
 */
export function NotFoundPage() {
  return (
    <Container size="sm" py="xl">
      <Stack align="center" gap="md" ta="center">
        <Compass size={64} strokeWidth={1.5} aria-hidden="true" />
        <Title order={1}>Page not found</Title>
        <Text c="dimmed" maw={420}>
          The page you are looking for doesn&apos;t exist or may have moved.
        </Text>
        <Button component={Link} to="/feed">
          Back to feed
        </Button>
      </Stack>
    </Container>
  );
}
