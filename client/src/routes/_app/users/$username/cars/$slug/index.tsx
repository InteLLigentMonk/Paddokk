import { createFileRoute, notFound } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import { isNotFoundError } from "@/lib/api/error-resolver";
import {
  carImagesQueryOptions,
  carJourneysQueryOptions,
  userCarBySlugQueryOptions,
  userCarsByUsernameQueryOptions,
} from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/cars/$slug/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      const car = await queryClient.ensureQueryData(
        userCarBySlugQueryOptions(params.username, params.slug),
      );
      await Promise.all([
        queryClient.ensureQueryData(carImagesQueryOptions(Number(car.id))),
        queryClient.prefetchQuery(
          userCarsByUsernameQueryOptions(params.username, 6),
        ),
        queryClient.prefetchQuery(
          carJourneysQueryOptions(params.username, params.slug),
        ),
      ]);
    } catch (error) {
      if (isNotFoundError(error)) throw notFound();
      throw error;
    }
  },
  component: UserCarDetailRoute,
});

function UserCarDetailRoute() {
  const { username, slug } = Route.useParams();

  const { data: car } = useQuery(userCarBySlugQueryOptions(username, slug));
  const { data: imagesResponse } = useQuery(
    carImagesQueryOptions(Number(car?.id ?? 0)),
  );

  if (!car) return null;

  return <CarDetailPage car={car} images={imagesResponse?.images ?? []} />;
}
