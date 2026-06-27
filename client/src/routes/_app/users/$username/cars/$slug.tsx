import { Outlet, createFileRoute, notFound } from "@tanstack/react-router";
import { userCarBySlugQueryOptions } from "@/lib/api/users.queries";
import { isNotFoundError } from "@/lib/api/error-resolver";

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
    } catch (error) {
      if (isNotFoundError(error)) throw notFound();
      throw error;
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
