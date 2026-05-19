import { createFileRoute, notFound, Outlet } from "@tanstack/react-router";
import { userCarBySlugQueryOptions } from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/cars/$slug")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      const car = await queryClient.ensureQueryData(
        userCarBySlugQueryOptions(params.username, params.slug),
      );
      const displayName =
        car.nickname ||
        (car.isCustomBuild
          ? (car.customBuildName ?? "Custom Build")
          : `${car.carMakeName ?? ""} ${car.carModelName ?? ""}`.trim()) ||
        params.slug;
      return { displayName };
    } catch {
      throw notFound();
    }
  },
  staticData: {
    breadcrumb: (loaderData) => {
      const data = loaderData as { displayName?: string } | undefined;
      return data?.displayName ?? "Car";
    },
  },
  component: () => <Outlet />,
});
