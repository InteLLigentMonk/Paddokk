import { createFileRoute, notFound } from "@tanstack/react-router";
import { Box } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { userJourneyBySlugQueryOptions } from "@/lib/api/users.queries";

export const Route = createFileRoute(
  "/_app/users/$username/journeys/$slug/v2/",
)({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      await queryClient.ensureQueryData(
        userJourneyBySlugQueryOptions(params.username, params.slug),
      );
    } catch (error) {
      throw notFound();
    }
  },
  component: JourneyDetailPageV2,
});

function JourneyDetailPageV2() {
  const { username, slug } = Route.useParams();

  const { data: journey } = useQuery(
    userJourneyBySlugQueryOptions(username, slug),
  );

  return (
    <Box
      pos="relative"
      style={{
        minHeight: 380,
        height: "min(54vh, 520px)",
        overflow: "hidden",
        background: journey?.primaryImageUrl
          ? `url(${journey.primaryImageUrl}) center/cover no-repeat`
          : "light-dark(var(--mantine-color-dark-7), var(--mantine-color-dark-8))",
      }}
    ></Box>
  );
}
