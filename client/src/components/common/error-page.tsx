import {
  Anchor,
  Button,
  Container,
  Group,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { RefreshCw, ServerCrash } from "lucide-react";
import { buildSupportHref } from "@/lib/support";
import { isApiError } from "@/lib/api/api-error";

interface ErrorPageProps {
  /** The caught error. Used to build a helpful support mail body. */
  error?: Error;
  /**
   * Retrigger the current route. Defaults to a full page reload when the
   * caller does not supply a router-aware handler.
   */
  onReload?: () => void;
}

/**
 * Full-page 500 state shown by the root `errorComponent` when a route loader
 * or component throws. Presentational: the boundary owns error reporting and
 * decides what "reload" does.
 */
export function ErrorPage({ error, onReload }: ErrorPageProps) {
  const handleReload = onReload ?? (() => window.location.reload());

  // The backend message is diagnostic and never shown (ADR-0007); the support link and
  // on-screen reference carry the correlation id instead, so a report ties to the log line.
  const traceId = isApiError(error) ? error.traceId : undefined;

  return (
    <Container size="sm" py="xl">
      <Stack align="center" gap="md" ta="center">
        <ServerCrash size={64} strokeWidth={1.5} aria-hidden="true" />
        <Title order={1}>Something went wrong</Title>
        <Text c="dimmed" maw={420}>
          An unexpected error occurred. You can try reloading the page, or let
          us know if the problem keeps happening.
        </Text>
        <Group justify="center" gap="sm">
          <Button
            leftSection={<RefreshCw size={16} strokeWidth={1.5} />}
            onClick={handleReload}
          >
            Reload
          </Button>
          <Anchor href={buildSupportHref("Problem report", traceId)}>
            Report a problem
          </Anchor>
        </Group>
        {traceId && (
          <Text c="dimmed" size="xs">
            Reference: {traceId}
          </Text>
        )}
      </Stack>
    </Container>
  );
}
