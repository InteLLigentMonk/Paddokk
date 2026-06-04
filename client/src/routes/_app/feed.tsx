import { createFileRoute } from "@tanstack/react-router";
import { Container, Stack, Title } from "@mantine/core";
import { feedInfiniteQueryOptions } from "@/lib/api/feed.queries";
import { FeedStream } from "@/components/feed/feed-stream";

export const Route = createFileRoute("/_app/feed")({
  // Anonymous visitors are already redirected to "/" by the _app parent layout,
  // so the feed loader only has to warm the first page (ADR-0004).
  loader: async ({ context: { queryClient } }) => {
    await queryClient.ensureInfiniteQueryData(feedInfiniteQueryOptions());
  },
  component: FeedPage,
});

function FeedPage() {
  return (
    <Container size="sm" py="xl">
      <Stack gap="md">
        <Title order={1}>Feed</Title>
        <FeedStream />
      </Stack>
    </Container>
  );
}
