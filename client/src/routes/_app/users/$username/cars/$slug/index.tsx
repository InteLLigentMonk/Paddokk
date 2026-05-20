import { createFileRoute, notFound } from "@tanstack/react-router";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import {
  userCarBySlugQueryOptions,
  carImagesQueryOptions,
  userCarsByUsernameQueryOptions,
} from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/cars/$slug/")({
  loader: async ({ params, context: { queryClient } }) => {
    try {
      const car = await queryClient.ensureQueryData(
        userCarBySlugQueryOptions(params.username, params.slug),
      );
      const [imagesResponse] = await Promise.all([
        queryClient.ensureQueryData(carImagesQueryOptions(Number(car.id))),
        queryClient.prefetchQuery(userCarsByUsernameQueryOptions(params.username)),
      ]);
      return { car, images: imagesResponse?.images ?? [] };
    } catch {
      throw notFound();
    }
  },
  component: UserCarDetailRoute,
});

function UserCarDetailRoute() {
  const { car, images } = Route.useLoaderData();
  return <CarDetailPage car={car} images={images} />;
}
