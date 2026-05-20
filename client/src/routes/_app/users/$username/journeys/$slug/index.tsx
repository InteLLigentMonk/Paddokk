import { useEffect } from "react";
import { createFileRoute, notFound } from "@tanstack/react-router";
import {
  Container,
  Stack,
  Text,
  Center,
  Alert,
  Skeleton,
  Group,
  Box,
} from "@mantine/core";
import { useIntersection } from "@mantine/hooks";
import { AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { useJourneyPostsInfinite } from "@/hooks/use-journey-detail";
import { JourneyDetailHeader } from "@/components/journeys/journey-detail-header";
import { JourneyCreatePostBar } from "@/components/journeys/journey-create-post-bar";
import { JourneyPostCard } from "@/components/journeys/journey-post-card";
import { PageBreadcrumbs } from "@/components/common/page-breadcrumbs";
import { userJourneyBySlugQueryOptions } from "@/lib/api/users.queries";

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

export const Route = createFileRoute("/_app/users/$username/journeys/$slug/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await queryClient.ensureQueryData(
        userJourneyBySlugQueryOptions(params.username, params.slug),
      );
    } catch {
      throw notFound();
    }
  },
  component: JourneyDetailPage,
});

function JourneyDetailPage() {
  const { username, slug } = Route.useParams();

  const { data: journey, error: journeyError } = useQuery(
    userJourneyBySlugQueryOptions(username, slug),
  );

  const journeyId = Number(journey?.id ?? 0);

  const {
    data: postsData,
    isLoading: postsLoading,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
    error: postsError,
  } = useJourneyPostsInfinite(journeyId);

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
        {journeyError && (
          <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
            Could not load journey. Please try again.
          </Alert>
        )}

        {!journeyError && journey && <JourneyDetailHeader journey={journey} />}

        {journey?.isOwner && <JourneyCreatePostBar journey={journey} />}

        {postsError && (
          <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
            Could not load posts. Please try again.
          </Alert>
        )}

        {postsLoading && !postsError && <PostsLoadingSkeleton />}

        {!postsLoading && !postsError && posts.length === 0 && (
          <Center py="xl">
            <Text c="dimmed">No posts yet.</Text>
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
