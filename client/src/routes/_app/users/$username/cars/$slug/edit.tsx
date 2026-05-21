import { createFileRoute, notFound, redirect } from "@tanstack/react-router";
import { CarDetailPage } from "@/components/cars/car-detail-page";
import {
  carImagesQueryOptions,
  currentUserQueryOptions,
  userCarBySlugQueryOptions,
  userCarsByUsernameQueryOptions,
} from "@/lib/api/users.queries";

export const Route = createFileRoute("/_app/users/$username/cars/$slug/edit")({
  staticData: { breadcrumb: "Edit" },
  loader: async ({ params, context: { queryClient } }) => {
    const me = await queryClient.ensureQueryData(currentUserQueryOptions());
    if (!me.username) throw redirect({ to: "/login" });

    try {
      const car = await queryClient.ensureQueryData(
        userCarBySlugQueryOptions(me.username, params.slug),
      );
      const [imagesResponse] = await Promise.all([
        queryClient.ensureQueryData(carImagesQueryOptions(Number(car.id))),
        queryClient.prefetchQuery(userCarsByUsernameQueryOptions(me.username)),
      ]);
      return { car, images: imagesResponse.images };
    } catch {
      throw notFound();
    }
  },
  component: EditCarRoute,
});

function EditCarRoute() {
  const { car, images } = Route.useLoaderData();
  return <CarDetailPage car={car} images={images} />;
}
