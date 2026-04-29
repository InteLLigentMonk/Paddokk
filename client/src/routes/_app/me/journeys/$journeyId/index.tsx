import { useEffect } from "react";
import { createFileRoute, Link } from "@tanstack/react-router";
import {
  Container,
  Stack,
  Button,
  Group,
  Text,
  Center,
  Alert,
  Skeleton,
  Box,
} from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { ArrowLeft, AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import {
  journeyDetailQueryOptions,
  useJourneyPostsInfinite,
} from "@/hooks/use-journey-detail";
import { JourneyDetailHeader } from "@/components/journeys/journey-detail-header";
import { JourneyPostCard } from "@/components/journeys/journey-post-card";

function PostsLoadingSkeleton() {
  return (
    <Group gap="xs" h={{ base: 180, md: 250 }}>
      <Skeleton h="100%" w={{ base: "100%", md: "58%" }} />
      <Stack gap="md" justify="start" h="100%" flex={1} visibleFrom="md">
        <Skeleton height="20%" width="80%" />
        <Skeleton height="20%" width="60%" />
        <Skeleton height="20%" width="90%" />
        <Skeleton height="40%" width="100%" />
      </Stack>
    </Group>
  );
}

export const Route = createFileRoute("/_app/me/journeys/$journeyId/")({
  loader: ({ params, context: { queryClient } }) =>
    queryClient.ensureQueryData(
      journeyDetailQueryOptions(Number(params.journeyId)),
    ),
  component: JourneyDetailPage,
});

function JourneyDetailPage() {
  const { journeyId } = Route.useParams();
  const id = Number(journeyId);

  const { data: journey, error: journeyError } = useQuery(
    journeyDetailQueryOptions(id),
  );

  const {
    data: postsData,
    isLoading: postsLoading,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
    error: postsError,
  } = useJourneyPostsInfinite(id);

  const { ref, entry } = useIntersection({ root: null, threshold: 0.1 });

  useEffect(() => {
    if (entry?.isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [entry?.isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const posts = postsData?.pages.flat() ?? [];

  return (
    <Container size="lg" py="xl">
      <Stack gap="xl">
        <Group>
          <Button
            component={Link}
            to="/me/journeys"
            variant="outline"
            leftSection={<ArrowLeft size={16} />}
          >
            Tillbaka till mina resor
          </Button>
        </Group>

        {journeyError && (
          <Alert icon={<AlertCircle size={16} />} title="Fel" color="red">
            Kunde inte ladda resan. Försök igen.
          </Alert>
        )}

        {!journeyError && journey && <JourneyDetailHeader journey={journey} />}

        {postsError && (
          <Alert icon={<AlertCircle size={16} />} title="Fel" color="red">
            Kunde inte ladda inlägg. Försök igen.
          </Alert>
        )}

        {postsLoading && !postsError && <PostsLoadingSkeleton />}

        {!postsLoading && !postsError && posts.length === 0 && (
          <Center py="xl">
            <Text c="dimmed">Inga inlägg ännu.</Text>
          </Center>
        )}

        <Stack gap="lg">
          {posts.map((post) => (
            <JourneyPostCard key={String(post.id)} post={post} />
          ))}
        </Stack>

        <Box ref={ref} h={1} aria-hidden />

        {hasNextPage && posts.length > 0 && <PostsLoadingSkeleton />}
        {isFetchingNextPage && <PostsLoadingSkeleton />}
      </Stack>
    </Container>
  );
}
