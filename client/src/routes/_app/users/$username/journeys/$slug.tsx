import { Outlet, createFileRoute, notFound } from "@tanstack/react-router";
import { userJourneyBySlugQueryOptions } from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/journeys/$slug")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      const journey = await queryClient.ensureQueryData(
        userJourneyBySlugQueryOptions(params.username, params.slug),
      );
      const displayName = journey.title || params.slug;
      return { displayName };
    } catch {
      throw notFound();
    }
  },
  staticData: {
    breadcrumb: (loaderData) => {
      const data = loaderData as { displayName?: string } | undefined;
      return data?.displayName ?? "Journey";
    },
  },
  component: () => <Outlet />,
});
